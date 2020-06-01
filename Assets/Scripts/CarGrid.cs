using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGrid : MonoBehaviour
{
    public MeshRenderer[] dots;
    public Material trackMaterial;
    public Material groundMaterial;
    public Material unknownMaterial;

    // Start is called before the first frame update
    void Start()
    {
        dots = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var dot in dots)
        {
            var worldPos = dot.transform.position;
            SurfaceType surfaceType = Map2Texture.singleton.GetSurface(worldPos);
            switch (surfaceType)
            {
                case SurfaceType.Track:
                    dot.material = trackMaterial;
                    break;
                case SurfaceType.Ground:
                    dot.material = groundMaterial;
                    break;
                default:
                    dot.material = unknownMaterial;
                    break;
            }
        }
    }
}
