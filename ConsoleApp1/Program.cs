﻿// See https://aka.ms/new-console-template for more information
public static class Progrom
{
    public class SS<Tpye>
    {
        public SS(Action<int> aaaaa)
        {

        }
         
    }
    public class M
    {
        private object name;
        public object Name
        {
            get
            {
                if (name == null)
                {
                    name = new SS<int>(OnSS);
                }
                return name;
            }
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
        protected void RaisePropertyChanged(string propertyName = null)
        {

        } 
    }
    static void Main(string[] args)
    {
        M s = null;
        if (s == null)
        {
            s = new M();
        }

        Console.WriteLine("Hello, World!");
    }
    public static void OnSS(int s)
    {

    }
}