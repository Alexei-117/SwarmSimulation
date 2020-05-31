using Swarm.Grid;
using Swarm.Scenario;
using Swarm.Swarm;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Swarm
{
    public class SystemManager : MonoBehaviour
    {
        [SerializeField] private List<string> RunningSystems;
        [SerializeField] private List<string> PerFrameSystems;

        /*Generic*/
        private static List<SystemBase> systems = new List<SystemBase>();
        private GenericInformation genericInformation;
        private EntityManager entityManager;
        private float accumulatedTime = 0;

        /*Spawners*/
        private SwarmSpawner swarmSpawner;
        private GridSpawner gridSpawner;
        private LightSpawner lightSpawner;

        void Start()
        {
            genericInformation = GetComponent<GenericInformation>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            GetSpawners();
            InitializeSpawners();

            genericInformation.SetEntityManager(entityManager);
            genericInformation.SetData();

            InitializeSystems();
            RunSystems();
        }

        private void GetSpawners()
        {
            swarmSpawner = GameObject.Find("SwarmSpawner").GetComponent<SwarmSpawner>();
            gridSpawner = GameObject.Find("GridSpawner").GetComponent<GridSpawner>();
            lightSpawner = GameObject.Find("LightSpawner").GetComponent<LightSpawner>();
        }

        private void InitializeSpawners()
        {
            InitializeSwarm();
            InitializeGrid();
            InitializeLight();
        }

        private void InitializeSwarm()
        {
            swarmSpawner.SetLayoutLimits(genericInformation.GetLayoutWidth(), genericInformation.GetLayoutHeight());
            swarmSpawner.SetEntityManager(entityManager);
            swarmSpawner.SetNumberOfAgents(genericInformation.NumberOfAgents);
            swarmSpawner.Initialize();
        }

        private void InitializeGrid()
        {
            gridSpawner.SetLayoutLimits(genericInformation.GetLayoutWidth(), genericInformation.GetLayoutHeight());
            gridSpawner.SetEntityManager(entityManager);
            gridSpawner.Initialize();
        }


        private void InitializeLight()
        {
            lightSpawner.SetEntityManager(entityManager);
            lightSpawner.Initialize();
        }

        private void InitializeSystems()
        {
            foreach (SystemBaseManageable system in systems)
            {
                if (RunningSystems.Contains(system.Name))
                {
                    system.InitializeData();
                }
            }
        }

        private void RunSystems()
        {
            foreach(SystemBaseManageable system in systems)
            {
                if(RunningSystems.Contains(system.Name))
                {
                    system.Enabled = true;
                    system.GenericInformation = genericInformation;
                    RunningSystems.Remove(system.Name);
                }
            }
        }

        void Update()
        {
            if (accumulatedTime > genericInformation.TimeStep)
            {
                RunPerFrameSystems();
                accumulatedTime = 0;
                return;
            }

            StopPerFrameSystems();
            accumulatedTime += Time.deltaTime;
        }

        private void RunPerFrameSystems()
        {
            foreach (SystemBaseManageable system in systems)
            {
                if (PerFrameSystems.Contains(system.Name))
                {
                    system.Enabled = true;
                }
            }
        }

        private void StopPerFrameSystems()
        {
            foreach (SystemBaseManageable system in systems)
            {
                if (PerFrameSystems.Contains(system.Name))
                {
                    system.Enabled = false;
                }
            }
        }

        public static void AddSystem(SystemBaseManageable system)
        {
            systems.Add(system);
        }

        private void OnDestroy()
        {
            RunningSystems.Clear();
            PerFrameSystems.Clear();
            systems.Clear();
        }
    }
}