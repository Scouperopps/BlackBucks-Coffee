using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Prototipo básico de cliente NPC para BlackBucks Coffee.
/// Flujo: entra -> camina al mostrador -> hace su pedido -> espera -> 
/// camina a una silla -> se sienta -> (más adelante: recibe pedido y se va).
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NPCCustomer : MonoBehaviour
{
    public enum State
    {
        Entrando,
        CaminandoAlMostrador,
        PidiendoOrden,
        EsperandoPedido,
        CaminandoASilla,
        Sentado,
        Saliendo
    }

    [Header("Referencias de la escena")]
    public Transform mostrador;      // punto donde el NPC hace fila / pide
    public Transform sillaAsignada;  // se asigna al spawnear (o se busca una libre)

    [Header("Config")]
    public float radioDeLlegada = 0.3f;
    public float tiempoParaPedir = 2f;      // simula el tiempo que tarda en "hablar"
    public float tiempoMaximoDeEspera = 30f; // paciencia antes de irse molesto

    private NavMeshAgent agent;
    private State estadoActual;
    private float temporizador;

    // Datos simples del pedido — luego esto puede ser un ScriptableObject o clase Order
    public string pedidoActual { get; private set; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        CambiarEstado(State.CaminandoAlMostrador);
    }

    void Update()
    {
        switch (estadoActual)
        {
            case State.CaminandoAlMostrador:
                if (LlegoADestino())
                    CambiarEstado(State.PidiendoOrden);
                break;

            case State.PidiendoOrden:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0f)
                {
                    GenerarPedido();
                    CambiarEstado(State.EsperandoPedido);
                }
                break;

            case State.EsperandoPedido:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0f)
                {
                    // Se le acabó la paciencia -> por ahora lo mandamos a sentarse igual
                    // (más adelante: aquí iría la lógica de "cliente molesto")
                    CambiarEstado(State.CaminandoASilla);
                }
                break;

            case State.CaminandoASilla:
                if (LlegoADestino())
                    CambiarEstado(State.Sentado);
                break;

            case State.Sentado:
                // Aquí luego: esperar a que el jugador le entregue el pedido
                break;
        }
    }

    void CambiarEstado(State nuevoEstado)
    {
        estadoActual = nuevoEstado;

        switch (nuevoEstado)
        {
            case State.CaminandoAlMostrador:
                IrHacia(mostrador.position);
                break;

            case State.PidiendoOrden:
                agent.isStopped = true;
                temporizador = tiempoParaPedir;
                Debug.Log($"{name}: pidiendo orden...");
                break;

            case State.EsperandoPedido:
                temporizador = tiempoMaximoDeEspera;
                Debug.Log($"{name}: esperando su pedido ({pedidoActual})");
                break;

            case State.CaminandoASilla:
                if (sillaAsignada != null)
                    IrHacia(sillaAsignada.position);
                else
                    Debug.LogWarning($"{name}: no tiene silla asignada.");
                break;

            case State.Sentado:
                agent.isStopped = true;
                Debug.Log($"{name}: sentado, esperando su orden en la mesa.");
                break;
        }
    }

    void IrHacia(Vector3 destino)
    {
        agent.isStopped = false;
        agent.SetDestination(destino);
    }

    bool LlegoADestino()
    {
        if (agent.pathPending) return false;
        return agent.remainingDistance <= radioDeLlegada &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
    }

    void GenerarPedido()
    {
        // Placeholder — luego esto puede tirar de una lista de recetas del juego
        string[] menu = { "Latte", "Capuchino", "Americano", "Frappé" };
        pedidoActual = menu[Random.Range(0, menu.Length)];
    }
}