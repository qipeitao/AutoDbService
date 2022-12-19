// See https://aka.ms/new-console-template for more information
using AutoDbService.DbPrism.Models;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Windows.Input;

public static class Progrom
{
    public class SS<Tpye>
    {
        public SS(Action<int> aaaaa)
        {

        }
         
    }
    public class M: EngineBindableBase
    {
        private ICommand name;
        public ICommand Name
        {
            get
            { 
                return name ??= new DelegateCommand(OnSS);
            }
        }
        public  void OnSS()
        {

        }
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                RaisePropertyChanged(nameof(Index));
            }
        } 
        public object Value { set; get; }
       
    }
    static void Main(string[] args)
    {
        M s = null;
        if (s == null)
        {
            s = new M();
        }

        Trace.WriteLine("Hello, World!");
    }
   
}
