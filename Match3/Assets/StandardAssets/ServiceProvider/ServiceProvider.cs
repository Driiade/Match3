/*Copyright(c) <2017> <Benoit Constantin ( France ) >

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System;
using System.Collections.Generic;

//https://digitalrune.github.io/DigitalRune-Documentation/html/619b1341-c6a1-4c59-b33d-cc1f799402dc.htm

namespace BC_Solution
{
    public class ServiceProvider
    {

        private static Dictionary<Type, List<object>> serviceDictionary = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Get the first service of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            List<object> services;
            serviceDictionary.TryGetValue(typeof(T), out services);

            if (services != null && services.Count > 0)
                return (T)services[0];
            else
            {
                foreach (List<object> serviceList in serviceDictionary.Values)
                {
                    foreach (var service in serviceList)
                    {
                        if (service is T)
                            return (T)service;
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// Get the first service of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>(System.Predicate<T> match)
        {
            List<object> services;
            serviceDictionary.TryGetValue(typeof(T), out services);

            if (services != null && services.Count > 0)
            {
                List<T> convertedServices = new List<T>();
                for (int i = 0; i < services.Count; i++)
                {
                    convertedServices.Add((T)services[i]);
                }

                return convertedServices.Find(match);
            }
            else
                return default(T);
        }

        /// <summary>
        /// Get the all services of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetServices<T>()
        {
            List<object> services;
            serviceDictionary.TryGetValue(typeof(T), out services);

            if (services != null)
            {
                List<T> convertedServices = new List<T>(services.Count);

                for (int i = 0; i < services.Count; i++)
                {
                    convertedServices.Add((T)services[i]);
                }

                return convertedServices;
            }
            else
                return null;
        }


        public static void AddService<T>(T service)
        {
            List<object> services;

            serviceDictionary.TryGetValue(service.GetType(), out services);
            if (services == null)
            {
                services = new List<object>();
                serviceDictionary.Add(service.GetType(), services);
            }

            if(!services.Contains(service))
                services.Add(service);
        }



        public static void RemoveService<T>(T service)
        {
            List<object> services;

            serviceDictionary.TryGetValue(service.GetType(), out services);
            if (services != null)
            {
                services.Remove(service);
            }
        }
    }
}
