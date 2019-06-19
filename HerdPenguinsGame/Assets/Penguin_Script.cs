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

    public Animation idle_left;
    public Animation idle_right;
    public Animation walk_left;
    public Animation walk_right;
    public Animation run_back;
    public Animation run_left;
    public Animation run_right;
    public Animator animator;

    public Rigidbody penguin;
    public Rigidbody player;
    public Collider penguin_collider;

    public float max_velocity;
    public bool has_collided_with_player;
    public bool has_entered_exhibit;
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

        //idle_left = gameObject.GetComponent<Animation>();
        //idle_right = gameObject.GetComponent<Animation>();
        //walk_left = gameObject.GetComponent<Animation>();
        //walk_right = gameObject.GetComponent<Animation>();
        //run_back = gameObject.GetComponent<Animation>();
        //run_left = gameObject.GetComponent<Animation>();
        //run_right = gameObject.GetComponent<Animation>();

        game_runner_script = FindObjectOfType<Game_Runner>();
        animator = gameObject.GetComponent<Animator>();
        penguin_collider = penguin.GetComponent<Collider>();

        penguin.useGravity = true;
        penguin.isKinematic = false;

        max_velocity = 7.0f;
        has_collided_with_player = false;

        int rand_idle = Random.Range(0, 1);
        if (rand_idle == 0)
        {
            animator.Play("walk_left");
        }
        else
        {
            animator.Play("walk_right");
        }

    }

    private void FixedUpdate()
    {
        //if (penguin.velocity.y == 0.0f)
        //{
        //    Debug.Log("ENTERED");
        //    ResetAnimations();
        //    animator.SetBool("idle_left_bool", true);
        //    time_on_ground++;

        //    if (time_on_ground > 650)
        //    {
        //        var local_velocity = transform.InverseTransformDirection(penguin.velocity);
        //        if (local_velocity.x < 0)
        //        {
        //            //moving left
        //            ResetAnimations();
        //            animator.SetBool("run_left_bool", true);
        //            penguin.AddForce(Vector3.forward * 450.0f);
        //        }
        //        else
        //        {
        //            //moving right
        //            ResetAnimations();
        //            animator.SetBool("run_left_bool", true);
        //            penguin.AddForce(Vector3.forward * 450.0f);
        //        }
        //    }
        //}
    }
    /*
     * called every frame
     */
    void Update()
    {
        var local_velocity = transform.InverseTransformDirection(penguin.velocity);
        
        if (penguin.velocity.y == 0.0f)
        {
            if (local_velocity.x < 0)
            {
                //moving left
                ResetAnimations();
                animator.SetBool("idle_left_bool", true);
                time_on_ground++;
            }
            else
            {
                //moving right
                ResetAnimations();
                animator.SetBool("idle_right_bool", true);
                time_on_ground++;
            }

            Debug.Log(time_on_ground);
            if (time_on_ground > 300)
            {
                time_on_ground = 0;
                if (local_velocity.x < 0)
                {
                    //moving left
                    ResetAnimations();
                    //animator.SetBool("run_left_bool", true);
                    //animator.SetBool("run_right_bool", true);
                    animator.Play("run_left");
                    Bounce(-3000.0f, 0.0f, 0.0f);
                    StartCoroutine(Delay(4.0f));
                    //Bounce(3000.0f, 3000.0f, 0.0f);
                }
                else
                {
                    //moving right
                    ResetAnimations();
                    //animator.SetBool("run_left_bool", true);
                    //animator.SetBool("run_right_bool", true);
                    animator.Play("run_right");
                    Bounce(3000.0f, 0.0f, 0.0f);
                    StartCoroutine(Delay(4.0f));
                    //Bounce(3000.0f, 3000.0f, 0.0f);
                }
            }
        }
        /*
         * this checks which direction the penguin is currently moving, and rotates the penguin
         * accordingly so that it is facing the direction it is moving
         */
        else
        {
            if (local_velocity.x < 0)
            {
                //moving left
                ResetAnimations();
                //Vector3 force = new Vector3(-5.0f, 0.0f, 0.0f);
                //penguin.AddForce(force);
                animator.SetBool("walk_left_bool", true);
            }
            else
            {
                //moving right
                ResetAnimations();
                //Vector3 force = new Vector3(5.0f, 0.0f, 0.0f);
                //penguin.AddForce(force);
                animator.SetBool("walk_right_bool", true);
            }
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
         * if a penguin goes too far on the left, a force is added to it to 
         * bounce it back towards the play area to ensure a penguin never goes offscreen.
         */
        if (penguin.position.x < -12.0f)
        {
            //too far left
            if (penguin.position.x < -12.0f)
            {
                Bounce(200.0f, 50.0f, 0.0f);
            }
        }
    }

    public IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /*
     * this method is called whenever a penguin collides with an object
     */
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Exhibit_Entrance")
        {
            if (has_collided_with_player)
            {
                //allows the penguin to enter the snow area because the collision with the invisible wall will be ignored
                Physics.IgnoreCollision(collision.collider, this.penguin_collider);
            }
            else
            {
                Bounce(1000.0f, 4.0f, 0.0f); //FIXME: change nums 
            }
        }

        //if the penguin collides with the player
        if (collision.gameObject.name == "Player")
        {
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
        if (collision.gameObject.tag == "Ground_Tag")
        {
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
            var local_velocity = transform.InverseTransformDirection(penguin.velocity);

            //variables for the current position of the penguin
            float cur_x = penguin.position.x;
            float cur_y = penguin.position.y;
            float cur_z = penguin.position.z;

            //current number of penguins in game
            int cur_num_penguins = game_runner_script.Get_Num_Penguins();

            if (OnTrackOne())
            {
                if (rand <= 5)
                {
                    //switch to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
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
            }
            else if (OnTrackThree())
            {
                if (rand <= 5)
                {
                    //switch to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
                }
            }

            float rand_y; //the new value for the y of the penguin
            rand_y = Random.Range(35.0f, 65.0f);

            //50% of the time, the penguin will bounce to the right
            if (rand <= 5)
            {
                //positive x value means bounce to the right
                Bounce(300.0f, rand_y, 0.0f);
            }
            //50% of the time, the penguin will bounce to the left
            else
            {
                //negative x value means bounce to the left
                Bounce(-300.0f, rand_y, 0.0f);
            }
        }

        if (collision.gameObject.tag == "Penguin_Tag")
        {
            int rand_num = RandomNum();
            if (rand_num <= 5)
            {
                Bounce(400.0f, 25.0f, 0.0f);
            }
            else
            {
                Bounce(-400.0f, 25.0f, 0.0f);
            }
        }

        if (collision.gameObject.name == "Snow")
        {
            Debug.Log("hit the snow");
            if (!has_entered_exhibit)
            {
                ResetAnimations();
                animator.SetBool("run_back_bool", true);
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
        animator.SetBool("run_left_bool", false);
        animator.SetBool("run_right_bool", false);
        animator.SetBool("run_back_bool", false);
        animator.SetBool("idle_left_bool", false);
        animator.SetBool("idle_right_bool", false);

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
