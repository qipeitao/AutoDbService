using AutoDbService.DbPrism.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoDbService.DbPrism.Models
{
    /// <summary>
    /// 维护对象列表
    /// 当对象不存在，释放关系维护
    /// </summary>
    public class DynamicTypeClear : IDynamicTypeClear
    {
        private ConcurrentDictionary<WeakReference<object>, object> keyValuePairs = new ConcurrentDictionary<WeakReference<object>, object>();
        private DispatcherTimer timer;
        public DynamicTypeClear()
        {
            timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
            keyValuePairs.Clear();
        }
        public bool AddObj(object obj,object type)
        {
          if (obj == null || type == null) return false;
           return keyValuePairs.TryAdd(new WeakReference<object>(obj), type); 
        }
        private void TimerOnTick(object? sender, EventArgs e)
        {
            keyValuePairs.Where(p => p.Key.TryGetTarget(out object _) == false)
                .ToList()
                .ForEach(p => 
                keyValuePairs.TryRemove(p.Key,out _)
                );
        }
    }
}
