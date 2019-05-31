using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Penguin_Script : MonoBehaviour
{
    //difficulty settings
    
    /* penguin_switch_num: this is the minimum number of penguins left in the game 
     * where switching to track two is allowed, when it is less than this, once a 
     * penguin is on track one it will never go to track two again
     */
    public int penguin_switch_num;

    public Animation anim;
    public Animator animator;

    public Rigidbody penguin;
    public Rigidbody player;
    public Collider penguin_collider;

    public float max_velocity;
    public bool has_collided_with_player;
    public int num_switches;
    public float time_on_ground;

    public Player_Script player_script;
    public Game_Runner game_runner_script;

    // Start is called before the first frame update
    void Start()
    {
        //initializes all of the variables that are used during execution
        player_script = player.GetComponent<Player_Script>();

        penguin = GetComponent<Rigidbody>(); 
        anim = gameObject.GetComponent<Animation>();
        game_runner_script = FindObjectOfType<Game_Runner>();
        animator = gameObject.GetComponent<Animator>();
        penguin_collider = penguin.GetComponent<Collider>();

        penguin.useGravity = true;
        penguin.isKinematic = false;

        max_velocity = 10.0f;
        has_collided_with_player = false;
    }

    /*
     * called every frame
     */
    void Update()
    {

        /*
         * this check ensures that the velocity of the penguin never exceeds the maximum so that 
         * the bouncing never becomes too extreme
         */
        if (penguin.velocity.magnitude > max_velocity)
        {
            penguin.velocity = max_velocity * penguin.velocity.normalized;
        }

        /*
         * if the penguin is positioned on or a little in front of the ramp, it will play the
         * panic animation which also changes the rotation of the penguin to face the henhouse,
         * this will hopefully cue the user that the penguin is correctly positioned to enter
         * the henhouse
         */
        if (penguin.position.x > 0.0 && OnTrackOne())
        {
            //ResetAnimations();
            //animator.SetBool("panic_bool", true);
        }
        else
        {
            //animator.SetBool("panic_bool", false);
            //animator.SetBool("flyagain_bool", true);
        }

        /*
         * if a penguin goes too far on either side of the screen, a force is added to it to 
         * bounce it back towards the play area to ensure a penguin never goes offscreen.
         */
        if (penguin.position.x > 6.5f || penguin.position.x < -7.5f)
        {
            //too far right
            if (penguin.position.x > 6.5f && OnTrackTwo())
            {
                Bounce(-300.0f, 75.0f, 0.0f);
            }

            //too far left
            else if (penguin.position.x < -7.5f)
            {
                Bounce(100.0f, 50.0f, 0.0f);
            }
        }
    }

    /*
     * this method is called whenever a penguin collides with an object
     */
    public void OnCollisionEnter(Collision collision)
    {

        //if the penguin collides with the player
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("collided with player");
            //has_collided_with_player = true; //sets flag and allows penguin to enter henhouse

            ///*
            // * only plays the animation 'shout' if the penguin is not in the location where it's
            // * oriented to enter the henhouse (i.e. if it's not currently playing 'panic', thus 
            // * not altering its new rotation
            // */
            //if (!(penguin.position.x > 0.0) && !OnTrackOne())
            //{
            //    ResetAnimations();
            //    animator.SetBool("shout_bool", true);
            //}

            //Bounce(2500.0f, 1000.0f, 0.0f); //bounces the penguin with a substantial amount of force when colliding with the user
        }

        //if the penguin collides with the grass
        if (collision.gameObject.name == "Grass")
        {
            Debug.Log("collided with grass");
            /*
             * penguins move between track one and track two randomly upon colliding with the ground, and 
             * only when they are on track one are they aligned properly to move into the henhouse.
             * 
             * from the view of the player, the tracks are the same, since their view is "2D", 
             * so it simply appears that the penguins sometimes are pushed right past the ramp
             * and sometimes they walk up it
             * 
             * track one: along the line z = -3.0f
             * track two: along the line z = -5.0f
             */

            //sets a random number from 1 (inclusive) to 10 (inclusive)
            int rand = RandomNum();

            //variables for the current position of the penguin
            float cur_x = penguin.position.x;
            float cur_y = penguin.position.y;
            float cur_z = penguin.position.z;

            //current number of penguins in game
            int cur_num_penguins = game_runner_script.Get_Num_Penguins();

            /* 
             * if penguin is on track two and is not further right than where the ramp starts,
             * this ensures that the penguin will not switch to track two and then get stuck under the ramp
             */
            if (OnTrackTwo() && cur_x <= 3.0f)
            {
                //if there are fewer than 6 penguins it will go to track one, otherwise 50% of the time penguin will switch
                if (cur_num_penguins < 6 || rand <= 5)
                {
                    //penguin switches to track one
                    penguin.transform.position = new Vector3(cur_x, cur_y, -3.0f);
                }
            }

            /* 
             * if penguin is on track one and there are more than four penguins currently in the game, this 
             * ensures that when the game is close to ending the penguins will mostly be oriented to
             * enter the henhouse
             */
            if (OnTrackOne())
            {
                //if there are more than 6 penguins it could switch 50% of the time
                if (cur_num_penguins > penguin_switch_num && rand <= 5)
                {
                    //penguin switches to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
                }

            }

            /*
             * changes the animation of the penguin with each collision with the ground, unless
             * the penguin is oriented to enter the henhouse, there's a 50% it will play 'pokpok'
             * and a 50% it will play 'cheer'
             */
            if (!animator.GetBool("panic_bool"))
            {
                ResetAnimations();
                if (rand <= 5)
                {
                    animator.SetBool("pokpok_bool", true);
                }
                else
                {
                    animator.SetBool("cheer_bool", true);
                }
            }

            float rand_y; //the new value for the y of the penguin

            int bounce_rand = RandomNum(); //random num for bounce from 1 (inclusive) to 10 (inclusive)

            //90% of the time, the penguin will bounce at a smaller interval, between 50 and 250
            if (bounce_rand <= 9)
            {
                rand_y = Random.Range(250.0f, 400.0f);
            }
            //10% of the time, the penguin will bounce at a larger interval, between 250 and 600
            else
            {
                rand_y = Random.Range(400.0f, 800.0f);
            }

            //50% of the time, the penguin will bounce to the right
            if (rand <= 5)
            {
                //positive x value means bounce to the right
                
                Bounce(500.0f, rand_y, 0.0f);
            }
            //50% of the time, the penguin will bounce to the left
            else
            {
                //negative x value means bounce to the left
                Bounce(-500.0f, rand_y, 0.0f);
            }
        }

        //if the penguin collides with the henhouse
        if (collision.gameObject.name == "Henhouse" && OnTrackOne())
        {
        //    /*
        //     * only if the penguin has collided with the player already is is able to enter the henhouse
        //     * and be destroyed, this ensures that the penguins will not enter the henhouse until
        //     * gameplay has started, and thus gives the actors ample time to set up the scene
        //     */
        //    if (has_collided_with_player)
        //    {
        //        //destroys the penguin
        //        Destroy(this.gameObject);

        //        //decrements the number of penguins in scene
        //        game_runner_script.Decrement_Num_Penguins();
        //    }
        }
    }


    /*
     * Generates a random number from 1 (inclusive) to 10 (inclusive)
     *
     * @return random integer
     */
    public int RandomNum()
    {
        int rand = Random.Range(1, 10);
        return rand;
    }

    /*
     * Resets the boolean values for all of the animations and thus causes
     * the animation controller to return to its default state
     */ 
    public void ResetAnimations()
    {
        return;
    }

    /*
     * Adds a force to the penguin based on the x, y, and z parameters that 
     * allows it to jump
     * 
     * @param x x-value
     * @param y y-value
     * @param z z-value
     */ 
    public void Bounce(float x, float y, float z)
    {
        penguin.AddForce(x, y, z);
    }

    /*
     * Checks which track the penguin is currently on and returns a boolean
     * based on that
     * 
     * @return whether the penguin is on track one or not
     */ 
    public bool OnTrackOne()
    {
        if (penguin.position.z == -3.0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
     * Checks which track the penguin is currently on and returns a boolean
     * based on that
     * 
     * @return whether the penguin is on track two or not
     */
    public bool OnTrackTwo()
    {
        if (penguin.position.z == -5.0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
