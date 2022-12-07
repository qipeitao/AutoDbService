using AutoDbService.DbPrism.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoDbService.DbPrism.Models
{
    public class BuildDynamicType: IBuildDynamicType
    { 
        private  MethodInfo RaisePropertyChangedInfo = typeof(BindableBase).GetRuntimeMethods().FirstOrDefault(p => p.Name == "RaisePropertyChanged");
        private IDynamicTypeClear dynamicTypeClear
        {
            get => AutoDbServiceEngine.Instance.Get<IDynamicTypeClear>();
        }
        private Type type;
        public TType BuildType<TType>() where TType : BindableBase
        {
            var  builder= BuildDynamicClass(typeof(TType));
             
            //////////////////////////////////////////////////////////
            type = builder.Item3.CreateType();
            TType obj=  builder.Item1.CreateInstance(type.FullName) as TType; 
            dynamicTypeClear.AddObj(obj, builder);
            return obj;
        }
        private  Tuple<AssemblyBuilder, ModuleBuilder, TypeBuilder> BuildDynamicClass(Type source)
        {
            AssemblyName myAsmName = new AssemblyName(source.Name+Guid.NewGuid().ToString("N"));// source.Assembly.GetName();
            string myModuleName = source.Name +"ModuleCLI";
            string myTypeName = source.Name + "TypeCLI";
            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.Run);
            //// Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(myModuleName);
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(myTypeName, TypeAttributes.Public, source);
            source.GetProperties().ToList().ForEach(p =>
            {
                myTypeBuilder = BuildDynamicClassWithProperties(myTypeBuilder, p.Name);
            });
            source.GetMethods().ToList().ForEach(p =>
            {
                if (p.DeclaringType == source && !p.Name.StartsWith("get_") && !p.Name.StartsWith("set_") && p.IsPublic && p.IsVirtual)
                {
                    myTypeBuilder = BuildDynamicClassWithCommands(myTypeBuilder, p); 
                }
            });
            return new Tuple<AssemblyBuilder, ModuleBuilder, TypeBuilder>(myAsmBuilder, myModBuilder, myTypeBuilder); 
        }
        private TypeBuilder BuildDynamicClassWithProperties(TypeBuilder myTypeBuilder, string propertyName)
        {
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField("my" + propertyName,
                                                            typeof(string),
                                                            FieldAttributes.Private | FieldAttributes.HasDefault);

            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(propertyName,
                                                             PropertyAttributes.None,
                                                            CallingConventions.HasThis,
                                                             typeof(string), null
                                                              );

            custNamePropBldr.GetRequiredCustomModifiers();

            //The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig
                    | MethodAttributes.Virtual;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder custNameGetPropMthdBldr =
                myTypeBuilder.DefineMethod("get_" + propertyName,
                                           getSetAttr,
                                           typeof(string),
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Nop);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Stloc_0);
            custNameGetIL.Emit(OpCodes.Br_S);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr =
                myTypeBuilder.DefineMethod("set_" + propertyName,
                                           getSetAttr,
                                           null,
                                           new Type[] { typeof(string) });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Nop);

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);

            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ldarg_0);

            custNameSetIL.Emit(OpCodes.Ldstr, propertyName);
            custNameSetIL.Emit(OpCodes.Call, RaisePropertyChangedInfo);
            custNameGetIL.Emit(OpCodes.Nop);
            custNameSetIL.Emit(OpCodes.Ret);

            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);

            return myTypeBuilder;
        }

        //private TypeBuilder BuildDynamicClassWithCommands(TypeBuilder myTypeBuilder, MethodInfo baseMethod)
        //{
        //    var argumentTypes = baseMethod.GetGenericArguments();
        //    if (argumentTypes.Length > 1)
        //    {
        //        return myTypeBuilder;
        //    }
        //    Type argumentActionType = typeof(Action<>);
        //    if (argumentTypes.Length > 0)
        //    {
        //        argumentActionType = argumentActionType.MakeGenericType(argumentTypes[0]);
        //    }
        //    Type argumentType = typeof(DelegateCommand<>);//
        //    if (argumentTypes.Length > 0)
        //    {
        //        argumentType = argumentType.MakeGenericType(argumentTypes[0]);
        //    }
        //    FieldBuilder customerNameBldr = myTypeBuilder.DefineField("my" + baseMethod.Name,
        //                                                    typeof(ICommand),
        //                                                    FieldAttributes.Private | FieldAttributes.HasDefault);

        //    PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(baseMethod.Name + "Command",
        //                                                     PropertyAttributes.None,
        //                                                    CallingConventions.HasThis,
        //                                                     typeof(ICommand), null);

        //    custNamePropBldr.GetRequiredCustomModifiers();

        //    //The property set and property get methods require a special
        //    // set of attributes.
        //    MethodAttributes getSetAttr =
        //        MethodAttributes.Public | MethodAttributes.SpecialName |
        //            MethodAttributes.HideBySig
        //            | MethodAttributes.Virtual;

        //    // Define the "get" accessor method for CustomerName.
        //    MethodBuilder custNameGetPropMthdBldr =
        //        myTypeBuilder.DefineMethod("get_" + baseMethod.Name + "Command",
        //                                   getSetAttr,
        //                                   typeof(ICommand),
        //                                   Type.EmptyTypes);
           
        //    ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();
        //    var labEndIf = custNameGetIL.DefineLabel();
        //    var labEnd = custNameGetIL.DefineLabel();
        //    custNameGetIL.Emit(OpCodes.Nop);
        //    custNameGetIL.Emit(OpCodes.Ldarg_0);
        //    custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
        //    custNameGetIL.Emit(OpCodes.Ldnull);
        //    custNameGetIL.Emit(OpCodes.Ceq);

        //    custNameGetIL.Emit(OpCodes.Stloc_0);
        //    custNameGetIL.Emit(OpCodes.Ldloc_0);
        //    custNameGetIL.Emit(OpCodes.Brfalse_S, labEndIf);
        //    custNameGetIL.Emit(OpCodes.Nop);
        //    custNameGetIL.Emit(OpCodes.Ldarg_0);
        //    custNameGetIL.Emit(OpCodes.Ldnull);
        //    custNameGetIL.Emit(OpCodes.Ldftn, baseMethod);

        //    custNameGetIL.Emit(OpCodes.Newobj, argumentActionType.GetConstructors().FirstOrDefault());
        //    custNameGetIL.Emit(OpCodes.Newobj, argumentType.GetConstructors().FirstOrDefault(p=>p.GetParameters().Length==1));

        //    custNameGetIL.Emit(OpCodes.Stfld, customerNameBldr);
        //    custNameGetIL.Emit(OpCodes.Nop);
        //    custNameGetIL.MarkLabel(labEndIf); 

        //    custNameGetIL.Emit(OpCodes.Ldarg_0);
        //    custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
        //    custNameGetIL.Emit(OpCodes.Stloc_1);
        //    custNameGetIL.Emit(OpCodes.Br_S, labEnd);
        //    custNameGetIL.MarkLabel(labEnd);
        //    custNameGetIL.Emit(OpCodes.Ldloc_1);
        //    custNameGetIL.Emit(OpCodes.Ret);

        //    custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
        //    return myTypeBuilder;
        //}
        private TypeBuilder BuildDynamicClassWithCommands(TypeBuilder myTypeBuilder, MethodInfo baseMethod)
        {
            var argumentTypes = baseMethod.GetGenericArguments();
            if (argumentTypes.Length > 1)
            {
                return myTypeBuilder;
            }
            Type argumentActionType = typeof(Action); 
            Type argumentType = typeof(DelegateCommand);// 
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField("my" + baseMethod.Name,
                                                            typeof(ICommand),
                                                            FieldAttributes.Private | FieldAttributes.HasDefault);

            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(baseMethod.Name + "Command",
                                                             PropertyAttributes.None,
                                                            CallingConventions.HasThis,
                                                             typeof(ICommand), null);

            custNamePropBldr.GetRequiredCustomModifiers();

            //The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig
                    | MethodAttributes.Virtual;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder custNameGetPropMthdBldr =
                myTypeBuilder.DefineMethod("get_" + custNamePropBldr.Name,
                                           getSetAttr,
                                           typeof(ICommand),
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();
            var labEndIf = custNameGetIL.DefineLabel();
            var labEnd = custNameGetIL.DefineLabel();
            custNameGetIL.Emit(OpCodes.Nop);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Dup);
            custNameGetIL.Emit(OpCodes.Brtrue_S, labEndIf);
            custNameGetIL.Emit(OpCodes.Pop);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldarg_0);

            custNameGetIL.Emit(OpCodes.Ldftn, baseMethod);
            custNameGetIL.Emit(OpCodes.Newobj, argumentActionType.GetConstructors().FirstOrDefault());
            custNameGetIL.Emit(OpCodes.Newobj, argumentType.GetConstructors().FirstOrDefault(p => p.GetParameters().Length == 1));

            custNameGetIL.Emit(OpCodes.Dup);
            custNameGetIL.Emit(OpCodes.Stloc_0);
            custNameGetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.MarkLabel(labEndIf);
            custNameGetIL.Emit(OpCodes.Stloc_1);
            custNameGetIL.Emit(OpCodes.Br_S, labEnd);
            custNameGetIL.MarkLabel(labEnd);
            custNameGetIL.Emit(OpCodes.Ldloc_1);
            custNameGetIL.Emit(OpCodes.Ret);
             
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr); 
            return myTypeBuilder;
        }
    }
}
