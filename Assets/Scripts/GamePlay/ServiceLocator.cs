using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IService 
{
    public void Start();
    public void End();

    public void Update();
}

public class ServiceLocator
{
    #region Singleton
    public static ServiceLocator Instance 
    { 
        get
            {
                if (m_instance == null)
                    m_instance = new ServiceLocator();
                
                return m_instance;
            }
        private set { } 
    }


    private static ServiceLocator m_instance;
    #endregion
    private ServiceLocator() 
    {
        m_services = new List<IService>();
        m_bInit = false;
    }

    private List<IService> m_services;
    private bool m_bInit;

    public void ProvideService(IService service )
    {
        m_services.Add(service);
    }
    
    public T GetService<T>() where T : IService 
    {
        foreach (IService s in m_services) 
        {
            Type serviceInterface = typeof(T);
            Type member = s.GetType();
            if (serviceInterface.Equals(member))
                return (T)s;
        }

        return default(T);
    }

    public void InitializeServices() 
    {
        if (m_bInit) 
        {
            Debug.Log("Services Already Initialized");
            return;//show message saying that the services are already initialized
        }

        foreach (var s in m_services) 
        {
            s.Start();
        }

        m_bInit = true;
    }

    public void UpdateServices() 
    {
        if (m_bInit && m_services != null)
        {
            foreach (var s in m_services)
                s.Update();
        }
    }
    public void ShutdownServices() 
    {
        if (m_bInit && m_services != null) 
        {
            foreach (var s in m_services)
                s.End();
        }
    }
}
