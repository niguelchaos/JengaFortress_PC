using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    //[SerializeField] private Player player;
    [SerializeField] public CurrentPlayer player { get; set; }
    private BoxCollider boundaryCollider;
    [SerializeField] private float boundaryOffset = 50.0f;
    //private Vector3 boundaryPos_P1, boundaryPos_P2;
    public bool isWithinBoundary { get; private set; }
    // [SerializeField] private GameObject groundPlaneGO;
    // private GameObject sessionOrigin;
    // private Setup setup;

    private MeshRenderer meshRenderer;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        // sessionOrigin = GameObject.Find("AR Session Origin");
        // setup = sessionOrigin.GetComponent<Setup>();
        
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        // if (gameState == GameState.SET_BOUNDARIES)
        //     SetBoundaryTransform();
        CheckMeshRenderer();
    }

    // todo: dont think the position are set correctly (might have something to do with ARSessionOrigin's scaling)
    public void SetBoundaryTransform(GameObject groundPlaneGO, float scaleFactor = 1.0f)
    {
        //GameObject groundPlaneGO = setup.groundPlane;

        // Debug.Log("groundPlaneGO.transform.localScale: " + groundPlaneGO.transform.localScale);
        // Debug.Log("groundPlaneGO.transform.localPosition: " + groundPlaneGO.transform.localPosition);
        // Debug.Log("groundPlaneGO.transform.position: " + groundPlaneGO.transform.localPosition);

        // todo: improve a lot on how boundaries are set
        // boundaryCollider.size = new Vector3 (
        Vector3 scaledGroundPlaneScale = groundPlaneGO.transform.localScale * scaleFactor;
        Vector3 scaledGroundPlanePosition = groundPlaneGO.transform.position * scaleFactor;
        float scaledBoundaryOffset = boundaryOffset * scaleFactor;

        transform.localScale = new Vector3(
            scaledGroundPlaneScale.x / 2 - scaledBoundaryOffset * 2,
            200f,
            scaledGroundPlaneScale.z - scaledBoundaryOffset * 2
        );

        // if (player == CurrentPlayer.PLAYER_1) {
        //     transform.position = groundPlaneGO.transform.position + new Vector3(
        //         -scaledGroundPlaneScale.x / (2 *  scaleFactor) + scaledBoundaryOffset,
        //         -0.5f,
        //         -scaledGroundPlaneScale.z / (2 *  scaleFactor) + scaledBoundaryOffset
        //     );
        // } else if (player == CurrentPlayer.PLAYER_2) {
        //     transform.position = groundPlaneGO.transform.position + new Vector3(
        //         scaledBoundaryOffset,
        //         -0.5f,
        //         -scaledGroundPlaneScale.z / (2 *  scaleFactor) + scaledBoundaryOffset
        //     );
        // }

        if (player == CurrentPlayer.PLAYER_1) {
            transform.position = groundPlaneGO.transform.position + new Vector3(
                -scaledGroundPlaneScale.x / (2 *  scaleFactor) + scaledBoundaryOffset,
                -0.5f,
                -scaledGroundPlaneScale.z / (2 *  scaleFactor) + scaledBoundaryOffset
            );
        } else if (player == CurrentPlayer.PLAYER_2) {
            transform.position = groundPlaneGO.transform.position + new Vector3(
                scaledBoundaryOffset,
                -0.5f,
                -scaledGroundPlaneScale.z / (2 *  scaleFactor) + scaledBoundaryOffset
            );
        }
    }

    public void CheckMeshRenderer()
    {
        DisableRenderer();
        if (GameManager.Instance.currentPlayer == player)
        {
            EnableRenderer();
        }
    }

    public void EnableRenderer()
    {
        meshRenderer.enabled = true;
    }
    public void DisableRenderer()
    {
        meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        // if (other.gameObject.tag == "Player") {}
        isWithinBoundary = true;
        DisableRenderer();
        //Debug.Log("OnTriggerEnter: " + other.gameObject.name);
    }
    private void OnTriggerExit(Collider other) {
        // if (other.gameObject.tag == "Player") {}
        isWithinBoundary = false;
        EnableRenderer();
        //Debug.Log("OnTriggerExit: " + other.gameObject.name);
    }

}
