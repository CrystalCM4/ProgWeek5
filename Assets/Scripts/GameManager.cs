using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Stats
{
    public string name;
    public string description;
    public int health;

    public Stats(string n, string d, int h){
        name = n;
        description = d;
        health = h;
    }
}

public class Entity  
{
    public Stats myStats;

    public Entity(string n, string d, int h)
    {
        myStats = new(n, d, h);
    }

    public virtual void TakePhysDamage(int mult){
        myStats.health -= 2 * mult;
    }
    public virtual void TakeMagDamage(int mult){
        myStats.health -= 2 * mult;
    }
}

public class Enemy1 : Entity
{
    public Enemy1(string n, string d, int h)
    : base(n, d, h){}

    public override void TakePhysDamage(int mult)
    {
        base.TakePhysDamage(mult * 2);
    }

    public override void TakeMagDamage(int mult)
    {
        base.TakePhysDamage(mult * 2);
    }
}

public class Enemy2 : Entity
{
    public Enemy2(string n, string d, int h)
    : base(n, d, h){}

    public override void TakePhysDamage(int mult)
    {
        base.TakePhysDamage(mult);
    }

    public override void TakeMagDamage(int mult)
    {
        base.TakePhysDamage(mult * 2);
    }
}

public class Enemy3 : Entity
{
    public Enemy3(string n, string d, int h)
    : base(n, d, h){}

    public override void TakePhysDamage(int mult)
    {
        base.TakePhysDamage(mult);
    }

    public override void TakeMagDamage(int mult)
    {
        if (!GameManager.fire)
        {
            base.TakeMagDamage(mult);
        }
        else 
        {
            base.TakeMagDamage(mult * 4);
        }
    }
}

public class Enemy4 : Entity
{
    public Enemy4(string n, string d, int h)
    : base(n, d, h){}

    public override void TakePhysDamage(int mult)
    {
        base.TakePhysDamage(mult);
    }

    public override void TakeMagDamage(int mult)
    {
        if (!GameManager.healing)
        {
            base.TakeMagDamage(mult);
        }
        else 
        {
            base.TakeMagDamage(mult * 100);
        }
    }
}

public class GameManager : MonoBehaviour
{
    
    List<Entity> round = new();
    Entity enemy1;
    Entity enemy2;
    Entity enemy3;
    Entity enemy4;

    public GameObject enemy;
    public GameObject player;
    public GameObject buttons;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI playerText;
    public int roundNum = 0;
    public int playerHealth = 10;
    public bool playerLoseHealth = false;
    public bool enemyKilled = false;
    public bool attacking = true;
    public static bool fire = false;
    public static bool healing = false;
    public float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string d1 = "Weak to anything.";
        string d2 = "Weak to magic.";
        string d3 = "Weak to magic. Especially weak to fire.";
        string d4 = "Weak to... the letter Z?";

        enemy1 = new Enemy1("Enemy 1", d1, 10);
        enemy2 = new Enemy2("Enemy 2", d2, 10);
        enemy3 = new Enemy3("Enemy 3", d3, 10);
        enemy4 = new Enemy4("Enemy 4", d4, 10);

        round.Add(enemy1);
        round.Add(enemy2);
        round.Add(enemy3);
        round.Add(enemy4);

        attacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        enemyText.text = round[roundNum].myStats.name + "(" + round[roundNum].myStats.health + ")\n"
        + '"' + round[roundNum].myStats.description + '"';

        playerText.text = "You (" + playerHealth + ")";


        if (!attacking){
            //timer
            timer -= Time.deltaTime;

            buttons.SetActive(false);
            PlayerTurn();
            EnemyTurn();
        }
        else {
            buttons.SetActive(true);
        }

        //if enemy defeated
        if (round[roundNum].myStats.health <= 0){
            enemyKilled = true;
            //enemy shouldnt attack
            timer = 3;

            //next enemy
            if (roundNum != round.Count - 1){
                roundNum += 1;
            }
        }

        if (round[round.Count - 1].myStats.health <= 0 || playerHealth <= 0){
            //end game
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }

    public void PlayerTurn(){
        if (timer <= 3 && timer > 2.5){
            player.transform.position = new Vector3(0,0.75f,-3);
        }

        if (timer <= 2.5){
            player.transform.position = new Vector3(0,0.75f,-7);
        }
    }

    public void EnemyTurn(){
        if (!enemyKilled){
            if (timer <= 1.5 && timer > 1){
                enemy.transform.position = new Vector3(0,1.75f,-4);
                if (playerLoseHealth){
                    playerHealth -= 2;
                    playerLoseHealth = false;
                }
            }
        }
        

        if (timer <= 1){
            enemy.transform.position = new Vector3(0,1.75f,0);
            attacking = true;
        }
    }

    //player abilities
    public void SwordSlash(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = false;
        healing = false;

        if (round[roundNum] == enemy1){
             enemy1.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakePhysDamage(2);
        }
       
        attacking = false;
        timer = 3;
    }

    public void MagicalCurse(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = false;
        healing = false;

        if (round[roundNum] == enemy1){
             enemy1.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakeMagDamage(2);
        }

        attacking = false;
        timer = 3;
    }

    public void BurningLight(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = true;
        healing = false;

        if (round[roundNum] == enemy1){
             enemy1.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakeMagDamage(2);
        }

        attacking = false;
        timer = 3;
    }

    public void ThrowRock(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = false;
        healing = false;

        if (round[roundNum] == enemy1){
             enemy1.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakePhysDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakePhysDamage(2);
        }

        attacking = false;
        timer = 3;
    }

    public void ElectroSpell(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = false;
        healing = false;

        if (round[roundNum] == enemy1){
             enemy1.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakeMagDamage(2);
        }

        attacking = false;
        timer = 3;
    }

    public void EatPizza(){
        playerLoseHealth = true;
        enemyKilled = false;
        fire = false;
        healing = true;

        if (round[roundNum] == enemy1){
             enemy1.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy2){
             enemy2.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy3){
             enemy3.TakeMagDamage(2);
        }
        else if (round[roundNum] == enemy4){
             enemy4.TakeMagDamage(2);
        }

        attacking = false;
        timer = 3;
    }

}
