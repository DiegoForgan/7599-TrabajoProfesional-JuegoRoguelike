using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProyectile : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject proyectilePrefab;
    public float proyectileForce = 10f;

    public void Shoot(){
        GameObject proyectile = Instantiate(proyectilePrefab,shootPoint.position,shootPoint.rotation);
        Rigidbody2D proyectileRigidBody = proyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(shootPoint.up * proyectileForce, ForceMode2D.Impulse);
    }
}
