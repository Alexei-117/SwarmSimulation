using Swarm;
using Swarm.Grid;
using Swarm.Movement;
using Swarm.Scenario;
using Swarm.Swarm;
using Unity.Entities;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    void Start()
    {
        genericInformation = GetComponent<GenericInformation>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        GetSpawners();
        InitializeSpawners();

        genericInformation.SetEntityManager(entityManager);
        genericInformation.SetData();

        GetSystems();
        InitializeSystems();
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

    private void GetSystems()
    {
        accumulateAgentsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<AccumulateAgentsSystem>();
        plotGridSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<PlotGridSystem>();

        agentCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<AgentCollisionSystem>();
        boundaryCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BoundaryCollisionSystem>();
        cleanCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<CleanCollisionSystem>();
        moveForwardSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<MoveForwardSystem>();
        restoreCollidedPositionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<RestoreCollidedPositionSystem>();

        decideMovementSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<DecideMovementSystem>();
        consumptionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<ConsumptionSystem>();
        gatherSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GatherSystem>();
        transferSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<TransferSystem>();
        findHighestGradientSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<FindHighestGradientSystem>();
    }

    private void InitializeSystems()
    {
        accumulateAgentsSystem.Enabled = RunAccumulateAgentsSystem;
        plotGridSystem.Enabled = RunPlotGridSystem;

        agentCollisionSystem.Enabled = RunAgentCollisionSystem;
        boundaryCollisionSystem.Enabled = RunBoundaryCollisionSystem;
        cleanCollisionSystem.Enabled = RunCleanCollisionSystem;
        moveForwardSystem.Enabled = RunMoveForwardSystem;
        restoreCollidedPositionSystem.Enabled = RunRestoreCollidedPositionSystem;

        decideMovementSystem.Enabled = RunDecideMovementSystem;
        consumptionSystem.Enabled = RunConsumptionSystem;
        InitializeGatherSystem(); 
        InitializeTransferSystem();
        findHighestGradientSystem.Enabled = RunFindHighestGradientSystem;
    }

    private void InitializeGatherSystem()
    {
        gatherSystem.Enabled = RunGatherSystem;
        gatherSystem.lightsTranslations = genericInformation.lightsTranslations;
        gatherSystem.lights = genericInformation.lights;
    }

    private void InitializeTransferSystem()
    {
        transferSystem.Enabled = RunTransferSystem;
    }

    private void Update()
    {
        if (accumulatedTime > genericInformation.TimeStep)
        {
            //decideMovementSystem.Enabled = true;
            //consumptionSystem.Enabled = true;
            //gatherSystem.Enabled = true;
            //transferSystem.Enabled = true;
            //findHighestGradientSystem.Enabled = true;

            accumulatedTime = 0;
            return;
        }

        //decideMovementSystem.Enabled = false;
        //consumptionSystem.Enabled = false;
        //gatherSystem.Enabled = false;
        //transferSystem.Enabled = false;
        //findHighestGradientSystem.Enabled = false;

        accumulatedTime += Time.deltaTime;
    }

    [Header("Grid systems")]
    [SerializeField] private bool RunAccumulateAgentsSystem;
    [SerializeField] private bool RunPlotGridSystem;

    [Header("Physics systems")]
    [SerializeField] private bool RunAgentCollisionSystem;
    [SerializeField] private bool RunBoundaryCollisionSystem;
    [SerializeField] private bool RunCleanCollisionSystem;
    [SerializeField] private bool RunMoveForwardSystem;
    [SerializeField] private bool RunRestoreCollidedPositionSystem;

    [Header("Swarm systems")]
    [SerializeField] private bool RunDecideMovementSystem;
    [SerializeField] private bool RunConsumptionSystem;
    [SerializeField] private bool RunGatherSystem;
    [SerializeField] private bool RunTransferSystem;
    [SerializeField] private bool RunFindHighestGradientSystem;

    /*Generic*/
    private GenericInformation genericInformation;
    private EntityManager entityManager;
    private float accumulatedTime = 0;

    /*Spawners*/
    private SwarmSpawner swarmSpawner;
    private GridSpawner gridSpawner;
    private LightSpawner lightSpawner;

    /*Systems*/
    private AccumulateAgentsSystem accumulateAgentsSystem;
    private PlotGridSystem plotGridSystem;

    private AgentCollisionSystem agentCollisionSystem;
    private BoundaryCollisionSystem boundaryCollisionSystem;
    private CleanCollisionSystem cleanCollisionSystem;
    private MoveForwardSystem moveForwardSystem;
    private RestoreCollidedPositionSystem restoreCollidedPositionSystem;

    private DecideMovementSystem decideMovementSystem;
    private ConsumptionSystem consumptionSystem;
    private GatherSystem gatherSystem;
    private TransferSystem transferSystem;
    private FindHighestGradientSystem findHighestGradientSystem;
}
