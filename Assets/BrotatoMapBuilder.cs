using UnityEngine;
using UnityEngine.Tilemaps;

public class BrotatoMapBuilder : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase groundTile;

    public int size = 100;

    void Start()
    {
        int half = size / 2;

        for (int x = -half; x < half; x++)
        {
            for (int y = -half; y < half; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }
    }
}
