using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklep_Internetowy.Infrastuctures
{
  public  interface ISessionManager
    {
        //czyszczenie elementu sesji 
        void Abandon();
        //pobieranie elementu z sesji 
        T Get<T>(string key);
        T Get<T>(string key, Func<T> createDefault);
        //ustawienie elementu w sesji 
        void Set<T>(string name, T value);
        T TryGet<T>(string key);
    }
}
