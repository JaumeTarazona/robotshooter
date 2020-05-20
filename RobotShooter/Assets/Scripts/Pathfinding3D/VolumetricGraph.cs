﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricGraph : MonoBehaviour
{
    [HideInInspector]
    public List<Node> Graph = new List<Node>();
    public LayerMask wallMask;
    public float distanceBtwNodes = 1;
    public int geometryLayer;
    public float erosionOffset;


    //Grid graph
    int gridX, gridY, gridZ;
    public Vector3 gridWorldSize = new Vector3(20, 20, 20);
    public float floorElevation = 1;
    public float maxRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CrearGridNodes()
    {
        Graph.Clear();
        GameObject[] objectsWithLayer = FindActiveGameObjectsWithLayer(geometryLayer);
        Collider c;
        int num;
        gridX = Mathf.RoundToInt(gridWorldSize.x / distanceBtwNodes);
        gridY = Mathf.RoundToInt(gridWorldSize.y / distanceBtwNodes);
        gridZ = Mathf.RoundToInt(gridWorldSize.z / distanceBtwNodes);
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    if (y > gridY * 0.2f)
                    {
                        num = Random.Range(1, 20);
                        if (num == 1)
                        {
                            Node n = new Node();
                            n.position = new Vector3(x * distanceBtwNodes + distanceBtwNodes / 2 - gridWorldSize.x / 2, y * distanceBtwNodes + floorElevation, z * distanceBtwNodes + distanceBtwNodes / 2 - gridWorldSize.z / 2);
                            n.isValid = true;
                            Graph.Add(n);
                        }
                    }
                    else
                    {
                        Vector3 pos = new Vector3(x * distanceBtwNodes + distanceBtwNodes / 2 - gridWorldSize.x / 2, y * distanceBtwNodes + floorElevation, z * distanceBtwNodes + distanceBtwNodes / 2 - gridWorldSize.z / 2);
                        Node n = new Node();
                        n.position = pos;
                        n.isValid = true;
                        Graph.Add(n);
                        //num = Random.Range(1, 5);
                        for (int i = 0; i < objectsWithLayer.Length; i++)
                        {
                            c = objectsWithLayer[i].GetComponent<Collider>();
                            if (c.bounds.Contains(pos))
                            {
                                Graph.Remove(n);
                                break;
                            }     
                        }                                                
                    }                    
                }
            }
        }
        Debug.Log(Graph.Count);
    }

    void CrearCircleGrid()
    {
        Graph.Clear();
        int num = 1;
        //for (int height = 0; height < gridWorldSize.y; height++)
        //{
            for (int radius = 0; radius < maxRadius; radius++)
            {
            //num = Mathf.RoundToInt(Mathf.Lerp(1, maxRadius, radius / maxRadius));
                
                Debug.Log(num);
                for (int i = 0; i < num; i++)
                {
                    float angle = i * Mathf.PI * 2f / num;
                    Node n = new Node();
                    n.position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    Graph.Add(n);
                }
                num = num * 2;
            }
        //}  
        Debug.Log(Graph.Count);
    }

    GameObject[] FindActiveGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer && goArray[i].activeInHierarchy)
            {
                goList.Add(goArray[i]);
            }
        }

        if (goList.Count == 0)
        {
            return null;
        }

        return goList.ToArray();
    }

    public void CrearNodes()
    {
        Graph.Clear();
        GameObject[] objectsWithLayer = FindActiveGameObjectsWithLayer(geometryLayer);
        Collider c;
        for (int i = 0; i < objectsWithLayer.Length; i++)
        {
            c = objectsWithLayer[i].GetComponent<Collider>();
            Vector3 dist = new Vector3(c.bounds.max.x - c.bounds.min.x + (erosionOffset * 2), c.bounds.max.y - c.bounds.min.y + (erosionOffset * 2), c.bounds.max.z - c.bounds.min.z + (erosionOffset * 2));
            int boxX = Mathf.RoundToInt(dist.x / distanceBtwNodes);
            int boxY = Mathf.RoundToInt(dist.y / distanceBtwNodes);
            int boxZ = Mathf.RoundToInt(dist.z / distanceBtwNodes);
            for (int x = 0; x <= boxX; x++)
            {
                for (int y = 0; y <= boxY; y++)
                {
                    for (int z = 0; z <= boxZ; z++)
                    {
                        if ((x == 0 || x == boxX || y == 0 || y == boxY || z == 0 || z == boxZ) && (y * distanceBtwNodes - dist.y / 2 + objectsWithLayer[i].transform.position.y) > 0)
                        {
                            Node pn = new Node();
                            pn.position = new Vector3(x * distanceBtwNodes - dist.x / 2 + objectsWithLayer[i].transform.position.x, y * distanceBtwNodes - dist.y / 2 + objectsWithLayer[i].transform.position.y, z * distanceBtwNodes - dist.z / 2 + objectsWithLayer[i].transform.position.z);
                            Graph.Add(pn);
                        }
                    }
                }
            }
        }
    }

    public void CrearConnexions()
    {
        foreach (Node pn in Graph)
        {
            foreach (Node successor in Graph)
            {/*
                if (pn.Connections.Count >= 25)
                {
                    break;
                }*/
                /*if (DistanceToTargetSquared(pn.position, successor.position) > 10.0f)
                {
                    continue;
                }*/

                Ray r = new Ray
                {
                    origin = pn.position,
                    direction = successor.position - pn.position
                };

                RaycastHit hit;
                if (!Physics.Raycast(r.origin, r.direction, out hit, (successor.position - pn.position).magnitude, wallMask.value))
                {
                    Connection c = new Connection();
                    //c.cost = Mathf.RoundToInt((successor.position - pn.position).magnitude);
                    c.cost = 1;
                    c.successor = successor;
                    pn.Connections.Add(c);
                }
            }
        }
    }

    float DistanceToTargetSquared(Vector3 me, Vector3 target)
    {
        return (target - me).sqrMagnitude;
    }

    public void OnDrawGizmosSelected()
    {
        if (Graph.Count > 0)
        {
            foreach (Node n in Graph)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(n.position, 0.2f);
            }
            /*foreach (Connection c in Graph[0].Connections)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(Graph[0].position, c.successor.position);
                Debug.Log(c.cost);
            }*/
        }
    }
}