using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public GameObject FX;
    public GameObject target;
    public bool IsCrit;
    public int damage;
    public float lifetime;
    public bool istargetPlayer;
    public bool DealDamage;

    // Start is called before the first frame update
    void Start()
    {
        if (target != null)
        {
            LeanTween.move(gameObject, target.transform.position, 1f).setOnComplete(() =>
            {
                if (FX != null)
                {
                    GameObject o = Instantiate(FX, target.transform.position, Quaternion.identity);
                }

                // if(target.GetComponent<Enemy>())
                //     target.GetComponent<Enemy>().TakeDamage(damage);
                // if(target.GetComponent<Character>()){
                //     if(target.GetComponent<Character>().IsEnemy)
                //         target.GetComponent<Character>().TakeDamage(damage);
                // }
                // if(target.GetComponent<BattleCardDisplay>())
                //     target.GetComponent<BattleCardDisplay>().DealDamage(damage);
                if (DealDamage)
                {
                    if (istargetPlayer)
                        PVPManager.Get().DealDamageToOpponent(damage);
                    if (target.GetComponent<BattleCardDisplay>())
                        target.GetComponent<BattleCardDisplay>().DealDamage(damage);
                }
                SpellManager.IsPetAttacking = false;
                Destroy(gameObject, 0.1f);
            });

        }

    }

    public void Update()
    {
        if (target == null)
        {
            lifetime += Time.deltaTime;
            if (lifetime > 1f)
            {
                Destroy(gameObject);
            }
        }

    }
}
