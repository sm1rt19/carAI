using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Map2Texture : MonoBehaviour
{
    public const float MAP_SIZE = 300f;
    public const int GRID_RESOLUTION = 600;

    public SurfaceType[,] SurfaceArray;

    public static Map2Texture singleton;


    public void Awake()
    {
        singleton = this;
    }

    public SurfaceType GetSurface(Vector3 worldPos)
    {
        return GetPixel(worldPos);
    }

    private SurfaceType GetPixel(Vector3 worldPos)
    {
        float gridSize = MAP_SIZE / GRID_RESOLUTION;
        var u = (worldPos.x + (MAP_SIZE / 2f)) / gridSize - 0.5f;
        var v = (worldPos.z + (MAP_SIZE / 2f)) / gridSize - 0.5f;
        try
        {
            return SurfaceArray[Mathf.RoundToInt(u), Mathf.RoundToInt(v)];
        } catch
        {
            return SurfaceType.Undefined;
        }
        
    }

    public Texture2D CreateTexture2D()
    {
        SurfaceArray = new SurfaceType[GRID_RESOLUTION, GRID_RESOLUTION];

        var texture = new Texture2D(GRID_RESOLUTION, GRID_RESOLUTION, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public void UpdateTexture2D(Texture2D texture)
    {
        for (int u = 0; u < texture.width; u++)
        {
            for (int v = 0; v < texture.height; v++)
            {
                var worldPos = GetWorldPos(u, v);
                var type = RayCast(worldPos);
                SurfaceArray[u, v] = type;
                var color = GetColorForSurfaceType(type);
                texture.SetPixel(u, v, color);
            }
        }
    }

    public Vector3 GetWorldPos(int u, int v)
    {
        float gridSize = MAP_SIZE / GRID_RESOLUTION;
        float x = gridSize * (u + 0.5f) - MAP_SIZE / 2f;
        float z = gridSize * (v + 0.5f) - MAP_SIZE / 2f;
        return new Vector3(x, 10f, z);
    }

    private Color GetColorForSurfaceType(SurfaceType type)
    {
        switch (type)
        {
            case SurfaceType.Track:
                return Color.gray;
            case SurfaceType.Ground:
                return Color.green;
            case SurfaceType.Undefined:
                return Color.black;
            default:
                throw new NotImplementedException();
        }
    }

    public SurfaceType RayCast(Vector3 worldPos)
    {
        int layerMask = LayerMask.GetMask("ground");
        var ray = new Ray(worldPos, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, layerMask))
        {
            if (hit.collider.CompareTag("Track"))
            {
                return SurfaceType.Track;
            }
            else
            {
                return SurfaceType.Ground;
            }
        }

        return SurfaceType.Undefined;
    }

    public void SaveTexture2D(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    // Start is called before the first frame update
    void Start()
    {
        Texture2D texture = CreateTexture2D();
        UpdateTexture2D(texture);
        SaveTexture2D(texture, "Assets/Texture.png");
        print("All done");
    }
}

public enum SurfaceType
{
    Undefined,
    Track,
    Ground
}