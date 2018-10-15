﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ShipSpawner.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Recalls ship to a chosen landing pad
//

public class ShipTeleporter : MonoBehaviour, IUsable {

    MeshRenderer meshTo;
    MeshRenderer meshFrom;

    float opacityTo;
    float opacityFrom;

    bool teleporting;

    public void Use() {
        if (!PlayerData.instance.teleporting) StartCoroutine("TeleportShip");
    }

	void Update () {
        if (teleporting) {
            opacityTo += Time.deltaTime / PlayerData.instance.shipTeleportTime;
            opacityFrom -= Time.deltaTime / PlayerData.instance.shipTeleportTime;

            SetShipOpacities(opacityTo, opacityFrom);
        }
    }

    void SetShipOpacities(float to, float from) {
        meshTo.material.color = new Color(meshTo.material.color.r, meshTo.material.color.g, meshTo.material.color.b, to);
        meshFrom.material.color = new Color(meshFrom.material.color.r, meshFrom.material.color.g, meshFrom.material.color.b, from);
    }

    IEnumerator TeleportShip() {
        teleporting = true;
        PlayerData.instance.teleporting = true;

        Rigidbody shipRB = FindObjectOfType<Ship>().GetComponent<Rigidbody>();
        shipRB.isKinematic = true;
        shipRB.velocity = Vector3.zero;
        shipRB.angularVelocity = Vector3.zero;
        shipRB.gameObject.SetActive(false);

        meshTo = Instantiate(PlayerData.instance.dematerializedShipPrefab, transform.parent.position, transform.parent.rotation).GetComponentInChildren<MeshRenderer>();
        meshFrom = Instantiate(PlayerData.instance.dematerializedShipPrefab, shipRB.transform.position, shipRB.transform.rotation).GetComponentInChildren<MeshRenderer>();

        opacityTo = 0f;
        opacityFrom = 1f;
        SetShipOpacities(opacityTo, opacityFrom);

        yield return new WaitForSeconds(PlayerData.instance.shipTeleportTime);

        teleporting = false;
        PlayerData.instance.teleporting = false;

        Destroy(meshTo.gameObject);
        Destroy(meshFrom.gameObject);

        shipRB.transform.position = transform.parent.position;
        shipRB.transform.rotation = transform.parent.rotation;
        shipRB.isKinematic = false;
        shipRB.gameObject.SetActive(true);
    }
}
