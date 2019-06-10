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
         * this checks which direction the penguin is currently moving, and rotates the penguin
         * accordingly so that it is facing the direction it is moving
         */ 
        var localVelocity = transform.InverseTransformDirection(penguin.velocity);
        if (localVelocity.x < 0)
        {
            //moving left
            ResetAnimations();
            animator.SetBool("walk_left_bool", true);
        }
        else
        {
            //moving right
            ResetAnimations();
            animator.SetBool("walk_right_bool", true);
        }

        /*
         * this check ensures that the velocity of the penguin never exceeds the maximum so that 
         * the bouncing never becomes too extreme
         */
        if (penguin.velocity.magnitude > max_velocity)
        {
            penguin.velocity = max_velocity * penguin.velocity.normalized;
        }

        /*
         * if a penguin goes too far on either side of the screen, a force is added to it to 
         * bounce it back towards the play area to ensure a penguin never goes offscreen.
         */
        if (penguin.position.x > 6.5f || penguin.position.x < -11.5f)
        {
            //too far right
            if (penguin.position.x > 6.5f && OnTrackTwo())
            {
                Bounce(-300.0f, 75.0f, 0.0f);
            }

            //too far left
            else if (penguin.position.x < -11.5f)
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
        if(collision.gameObject.name == "Exhibit_Entrance" && has_collided_with_player)
        {
            //allows the penguin to enter the snow area because the collision with the invisible wall will be ignored
            Physics.IgnoreCollision(collision.collider, this.penguin_collider);
        }

        //if the penguin collides with the player
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("collided with player");
            has_collided_with_player = true; //sets flag and allows penguin to enter henhouse

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
        if (collision.gameObject.name == "Grass" || collision.gameObject.name == "Sidewalk")
        {
            Debug.Log("collided with grass or sidewalk");
            /*
             * penguins move between track one, two and three randomly upon colliding with the ground, and 
             * only when they are on track one or two are they aligned properly to be affected by the user
             * 
             * from the view of the player, the tracks are the same, since their view is "2D", 
             * so it simply appears that the penguins sometimes are pushed right past the ramp
             * and sometimes they walk up it
             * 
             * track one: along the line z = -7.0f
             * track two: along the line z = -5.0f
             * track three: along the line z = -3.0f
             */

            //sets a random number from 1 (inclusive) to 10 (inclusive)
            int rand = RandomNum();

            //variables for the current position of the penguin
            float cur_x = penguin.position.x;
            float cur_y = penguin.position.y;
            float cur_z = penguin.position.z;

            //current number of penguins in game
            int cur_num_penguins = game_runner_script.Get_Num_Penguins();
            
            if (OnTrackOne())
            {
                if(rand <= 3)
                {
                    //switch to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
                }
                else if(rand <= 7)
                {
                    //switch to track three if there are more than the specified number of penguins
                    if (cur_num_penguins > penguin_switch_num)
                    {
                        penguin.transform.position = new Vector3(cur_x, cur_y, -3.0f);
                    }
                }
                else if(rand <= 10)
                {
                    //stay on track one
                }

            }
            else if (OnTrackTwo())
            {
                if (rand <= 3)
                {
                    //switch to track one
                    penguin.transform.position = new Vector3(cur_x, cur_y, -7.0f);
                }
                else if (rand <= 7)
                {
                    //switch to track three if there are more than the specified number of penguins
                    if (cur_num_penguins > penguin_switch_num)
                    {
                        penguin.transform.position = new Vector3(cur_x, cur_y, -3.0f);
                    }
                }
                else if (rand <= 10)
                {
                    //stay on track two
                }

            }
            else if (OnTrackThree())
            {
                if (rand <= 3)
                {
                    //switch to track one
                    penguin.transform.position = new Vector3(cur_x, cur_y, -7.0f);
                }
                else if (rand <= 7)
                {
                    //switch to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
                }
                else if (rand <= 10)
                {
                    //stay on track three
                }

            }

            float rand_y; //the new value for the y of the penguin
            rand_y = Random.Range(100.0f, 200.0f);

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
        animator.SetBool("walk_left_bool", false);
        animator.SetBool("walk_right_bool", false);
        animator.SetBool("run_bool", false);

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
        if (penguin.position.z == -7.0)
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

    /*
     * Checks which track the penguin is currently on and returns a boolean
     * based on that
     * 
     * @return whether the penguin is on track three or not
     */
    public bool OnTrackThree()
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
}
