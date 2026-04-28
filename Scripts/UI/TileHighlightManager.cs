using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHighlightManager : MonoBehaviour
{
    [SerializeField] private Transform cellAnchor;
    [SerializeField] private GridLayout worldGrid;
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Transform playerTransform;

    private Vector2Int facingDirection = Vector2Int.down;
    private Vector3Int previousCell;
    private bool hasPreviousCell;

    private void Start()
    {
        Debug.Log("TileHighlightManager started");

        if (highlightTilemap == null) Debug.LogError("Highlight Tilemap is missing");
        if (highlightTile == null) Debug.LogError("Highlight Tile is missing");
        if (worldGrid == null) Debug.LogError("WorldGrid is missing");
        if (cellAnchor == null) Debug.LogError("CellAnchor is missing");
    }

    private void Update()
    {
        UpdateFacingDirection();
        UpdateHighlight();
    }

    private void UpdateFacingDirection()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            facingDirection = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            facingDirection = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            facingDirection = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            facingDirection = Vector2Int.right;
    }

    public Vector3Int GetTargetCell()
    {
        Vector3 worldPos = cellAnchor != null ? cellAnchor.position : playerTransform.position;

        Vector3Int playerCell = worldGrid.WorldToCell(worldPos);
        Vector3Int targetCell = playerCell + new Vector3Int(facingDirection.x, facingDirection.y, 0);

        return targetCell;
    }


    private void UpdateHighlight()
    {
        Vector3 worldPos = cellAnchor.position;
        Vector3Int playerCell = worldGrid.WorldToCell(worldPos);
        Vector3Int targetCell = playerCell + new Vector3Int(facingDirection.x, facingDirection.y, 0);

        if (hasPreviousCell)
            highlightTilemap.SetTile(previousCell, null);

        highlightTilemap.SetTile(targetCell, highlightTile);

        previousCell = targetCell;
        hasPreviousCell = true;

        Debug.Log($"Anchor world = {worldPos}, Player cell = {playerCell}, Target cell = {targetCell}");
    }
}