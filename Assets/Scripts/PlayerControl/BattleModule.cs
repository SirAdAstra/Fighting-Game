using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModule : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject swordModel;
    [SerializeField] private BoxCollider swordAttackCollider;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject pistolModel;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject rifleModel;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private GameObject shield;
    private enum Equipped {none, sword, pistol, rifle}
    private Equipped equipped;
    private bool attacking = false;
    private bool swordInInventory = false;
    private bool pistolInInventory = false;
    private bool rifleInInventory = false;
    private bool block = false;

    public float health = 123;
    public int damage;
    public int uses = 0;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            block = true;
            shield.SetActive(true);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            block = false;
            shield.SetActive(false);
        }

        if (Input.GetButton("Fire1") && !block)
        {
            switch(equipped)
            {
                case Equipped.none:

                    break;
                
                case Equipped.sword:
                    if (!attacking)
                        StartCoroutine(SwordAttack());
                    break;
                
                case Equipped.pistol:
                    if (!attacking)
                        StartCoroutine(PistolShot());
                    break;

                case Equipped.rifle:
                    if (!attacking)
                        StartCoroutine(RifleShot());
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && swordInInventory)
            SwitchToSword();
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && pistolInInventory)
            SwitchToPistol();
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && rifleInInventory)
            SwitchToRifle();

        if (Input.GetKeyDown(KeyCode.Q))
            Unequip(true);
    }

    private IEnumerator SwordAttack()
    {
        attacking = true;
        sword.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.09f);
        swordAttackCollider.enabled = true;
        yield return new WaitForSeconds(0.01f);
        swordAttackCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        uses -= 1;
        if (uses <= 0)
            Unequip(true);
        attacking = false;
    }

    private IEnumerator PistolShot()
    {
        attacking = true;
        Physics.Raycast(pistol.transform.position, pistol.transform.forward, out RaycastHit hit);
        ShotImpact(hit);
        pistol.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.6f);
        uses -= 1;
        if (uses <= 0)
            Unequip(true);
        attacking = false;
    }

    private IEnumerator RifleShot()
    {
        attacking = true;
        Physics.Raycast(rifle.transform.position, rifle.transform.forward, out RaycastHit hit);
        ShotImpact(hit);
        rifle.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.3f);
        uses -= 1;
        if (uses <= 0)
            Unequip(true);
        attacking = false;
    }

    private void ShotImpact(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "Enemy")
        {
            hit.collider.gameObject.GetComponent<AIController>().RecieveDamage(damage);
        }
        Instantiate(impactEffect, hit.point, Quaternion.Inverse(transform.rotation));
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.tag == "Sword")
            {
                sword.GetComponentInChildren<Sword>().usesRemain = sword.GetComponentInChildren<Sword>().uses;
                SwitchToSword();
                Destroy(other.gameObject);
            }

            if (other.tag == "Pistol")
            {
                pistol.GetComponentInChildren<Pistol>().usesRemain = pistol.GetComponentInChildren<Pistol>().uses;
                SwitchToPistol();
                Destroy(other.gameObject);
            }

            if (other.tag == "Rifle")
            {
                rifle.GetComponentInChildren<Rifle>().usesRemain = rifle.GetComponentInChildren<Rifle>().uses;
                SwitchToRifle();
                Destroy(other.gameObject);
            }

            if (other.tag == "spawner")
                other.GetComponent<ItemSpawner>().isEmpty = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyAttack")
            RecieveDamage(other.gameObject.GetComponentInParent<AIController>().damage);
    }
    
    private void SwitchToSword()
    {
        Unequip(false);
        equipped = Equipped.sword;
        damage = sword.GetComponentInChildren<Sword>().damage;
        uses = sword.GetComponentInChildren<Sword>().usesRemain;
        sword.SetActive(true);
        swordInInventory = false;
    }

    private void SwitchToPistol()
    {
        Unequip(false);
        equipped = Equipped.pistol;
        damage = pistol.GetComponentInChildren<Pistol>().damage;
        uses = pistol.GetComponentInChildren<Pistol>().usesRemain;
        pistol.SetActive(true);
        pistolInInventory = false;
    }

    private void SwitchToRifle()
    {
        Unequip(false);
        equipped = Equipped.rifle;
        damage = rifle.GetComponentInChildren<Rifle>().damage;
        uses = rifle.GetComponentInChildren<Rifle>().usesRemain;
        rifle.SetActive(true);
        rifleInInventory = false;
    }

    private void Unequip(bool dropOut)
    {
        switch(equipped)
            {
                case Equipped.none:
                    break;
                
                case Equipped.sword:
                    sword.SetActive(false);
                    if (dropOut)
                    {
                        Instantiate(swordModel, gameObject.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        sword.GetComponentInChildren<Sword>().usesRemain = uses;
                        swordInInventory = true;
                    }
                    break;
                
                case Equipped.pistol:
                    pistol.SetActive(false);
                    if (dropOut)
                    {
                        Instantiate(pistolModel, gameObject.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        pistol.GetComponentInChildren<Pistol>().usesRemain = uses;
                        pistolInInventory = true;
                    }
                    break;

                case Equipped.rifle:
                    rifle.SetActive(false);
                    if (dropOut)
                    {
                        Instantiate(rifleModel, gameObject.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        rifle.GetComponentInChildren<Rifle>().usesRemain = uses;
                        rifleInInventory = true;
                    }
                    break;
            }
        equipped = Equipped.none;
        damage = 0;
        uses = 0;
    }

    public void RecieveDamage(int incDamage)
    {
        if (!block)
            health -= incDamage;
        if (health <= 0)
        {
            transform.position = startPosition;
            health = 123;
        }
    }
}
