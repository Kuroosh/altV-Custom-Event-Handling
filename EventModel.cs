using AltV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VenoXV._RootCore_.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class VenoXRemoteEventAttribute : Attribute
    {
        public string Name { get; }
        public VenoXRemoteEventAttribute(string name = null)
        {
            Name = name;
        }
    }
    public class EventAssets : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerEvent)]
        public static void OnServerEventReceive(VnXPlayer player, string EventName, params object[] args)
        {
            try
            {
                /* Debug */
                //Core.Debug.OutputDebugStringColored("Called [OnServerEventReceive]", ConsoleColor.Green);

                var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
               .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
               .Where(x => x.IsClass) // only yields classes
               .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
               .Where(x => x.GetCustomAttributes(typeof(VenoXRemoteEventAttribute), false).FirstOrDefault() != null); // returns only methods that have VenoXRemoteEventAttribute

                foreach (MethodInfo method in methods)
                {
                    object[] _Attr = method.GetCustomAttributes(typeof(VenoXRemoteEventAttribute), false);
                    if (_Attr is not null && _Attr.Length > 0)
                    {
                        VenoXRemoteEventAttribute __obj = (VenoXRemoteEventAttribute)_Attr[0];
                        if (__obj is not null && __obj.Name == EventName)
                        {

                            /* Variables */
                            List<object> objList = new List<object> { player };
                            ParameterInfo[] __MethodParameters = method.GetParameters();
                            int i = 1;

                            /* Debug */
                            //Core.Debug.OutputDebugStringColored("Called EventName : [" + EventName + "]", ConsoleColor.Green);
                            //Core.Debug.OutputDebugString("[ServerEvent] : [" + player.Name + "] | [" + player.Username + "] called EventName : " + EventName + " | Args : " + string.Join(", ", args));

                            // Creating a new Instance
                            object __Instance = Activator.CreateInstance(method.DeclaringType);

                            // Fix - obj value types.
                            foreach (object __v in args) { objList.Add(Convert.ChangeType(__v, __MethodParameters[i].ParameterType)); i++; }

                            //Convert our list to a obj-Array.
                            object[] builder = objList.ToArray();
                            // invoke the method
                            method.Invoke(__Instance, builder);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { Core.Debug.CatchExceptions(ex, "OnServerEventReceive - " + EventName); }
        }
    }
}
