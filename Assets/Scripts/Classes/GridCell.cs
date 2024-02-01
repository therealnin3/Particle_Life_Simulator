using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector2 position;
    public List<Particle> particles = new List<Particle>();

    public GridCell(Vector2 position)
    {
        this.position = position;
    }
}
