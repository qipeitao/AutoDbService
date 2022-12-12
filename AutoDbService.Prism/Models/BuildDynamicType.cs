using AutoDbService.DbPrism.Attributes;
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
        private  MethodInfo? RaisePropertyChangedInfo = typeof(BindableBase).GetRuntimeMethods().FirstOrDefault(p => p.Name == "RaisePropertyChanged");
        private MethodInfo? SetCommandWhenNullInfo = typeof(EngineBindableBase).GetRuntimeMethods().FirstOrDefault(p => p.Name == "SetCommandWhenNull");
        
        private IPropertyAndCommandConvertName PropertyAndCommandConvertName {
            get => AutoDbServiceEngine.Instance.Get<IPropertyAndCommandConvertName>();
        }
        public object BuildType(Type baseType)
        {
            var builder = BuildDynamicClass(baseType);

            //////////////////////////////////////////////////////////
            var type = builder.Item3.CreateType();
            if (type == null)
            {
                throw new Exception("创建失败!");
            }
            object? obj = Activator.CreateInstance(type);
            if (obj == null)
            {
                throw new Exception("创建失败!");
            }
            //////默认初始化赋值
            if (SetCommandWhenNullInfo != null)
            {
                type.GetRuntimeFields().Where(p => p.IsPublic == false && PropertyAndCommandConvertName.IsMyCommandField(p.Name))
                     .ToList().ForEach(p =>
                     {
                         var method = type.GetMethod(PropertyAndCommandConvertName.GetMethodNameByFieldCommandName(p.Name));
                         if (method != null)
                         {
                             SetCommandWhenNullInfo.Invoke(obj, new object[] { p.Name, method });
                         }
                     });
            }
            type.GetRuntimeProperties() 
                .Where(p=>p.CanWrite&&p.CanRead&&p.SetMethod!=null&&p.GetMethod!=null).ToList()
                .ForEach(p =>
            {
                if(AutoDbServiceEngine.Instance.IsRegister(p.PropertyType))
                {
                    p.SetValue(obj, AutoDbServiceEngine.Instance[p.PropertyType]);
                }
            });
            return obj;
        }
        public TType BuildType<TType>() where TType : BindableBase
        { 
            return BuildType(typeof(TType)) as TType;
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
            source.GetProperties().Where(p=>p.GetCustomAttribute<BindingPropertyAttribute>()!=null)
                .ToList().ForEach(p =>
            {
                myTypeBuilder = BuildDynamicClassWithProperties(myTypeBuilder, p);
            });
            source.GetMethods().Where(p => p.GetCustomAttribute<CommandAttribute>()!=null)
                .ToList().ForEach(p =>
            {
                myTypeBuilder = BuildDynamicClassWithCommands(myTypeBuilder, p);
            }); 
            return new Tuple<AssemblyBuilder, ModuleBuilder, TypeBuilder>(myAsmBuilder, myModBuilder, myTypeBuilder); 
        }
        private TypeBuilder BuildDynamicClassWithProperties(TypeBuilder myTypeBuilder, PropertyInfo propertyInfo)
        {
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField(PropertyAndCommandConvertName.GetFieldByProperty(propertyInfo.Name),
                                                            propertyInfo.PropertyType,
                                                            FieldAttributes.Private | FieldAttributes.HasDefault);

            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(propertyInfo.Name,
                                                             PropertyAttributes.None,
                                                            CallingConventions.HasThis,
                                                              propertyInfo.PropertyType, null
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
                myTypeBuilder.DefineMethod("get_" + custNamePropBldr.Name,
                                           getSetAttr,
                                            propertyInfo.PropertyType,
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
                myTypeBuilder.DefineMethod("set_" + custNamePropBldr.Name,
                                           getSetAttr,
                                           null,
                                           new Type[] { propertyInfo.PropertyType });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Nop);

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);

            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ldarg_0);

            custNameSetIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
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
        //    FieldBuilder customerNameBldr = myTypeBuilder.DefineField(FieldPreName + baseMethod.Name,
        //                                                    typeof(ICommand),
        //                                                    FieldAttributes.Private | FieldAttributes.HasDefault);

        //    PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(baseMethod.Name + FieldLastName,
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
        //        myTypeBuilder.DefineMethod("get_" + baseMethod.Name + FieldLastName,
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
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField(PropertyAndCommandConvertName.GetFieldCommandByMethod(baseMethod.Name),
                                                            typeof(ICommand),
                                                            FieldAttributes.Private | FieldAttributes.HasDefault); 
            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(PropertyAndCommandConvertName.GetPropertyCommandByMethod(baseMethod.Name),
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
          
            custNameGetIL.Emit(OpCodes.Nop);
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr); 
            custNameGetIL.Emit(OpCodes.Ret);
             
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr); 
            return myTypeBuilder;
        }
    }
}
