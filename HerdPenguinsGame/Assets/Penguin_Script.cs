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
    public Animation animation;

    public Rigidbody penguin;
    public Rigidbody player;
    public Collider penguin_collider;

    public float max_velocity;
    public bool has_collided_with_player;
    public int num_switches;
    public float time_on_ground;
    public string direction;
    public bool is_in_pen;
    public bool ran_back_already;
    //public int num_penguins_in_pen;
    public float timer;

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
        animation = gameObject.GetComponent<Animation>();
        penguin_collider = penguin.GetComponent<Collider>();

        penguin.useGravity = true;
        penguin.isKinematic = false;

        max_velocity = 4.0f;
        has_collided_with_player = false;
        direction = "";
        is_in_pen = false;
        ran_back_already = false;
        penguin_switch_num = 4;
        timer = 0.0f;

        int rand_walk = Random.Range(0, 1);
        if (rand_walk == 0)
        {
            animator.Play("walk_left");
        }
        else
        {
            animator.Play("walk_right");
        }
    }

    /*
     * called every frame
     */
    void Update()
    {
        //num_penguins_in_pen = game_runner_script.Get_Num_Penguins_In_Pen();
        var local_velocity = transform.InverseTransformDirection(penguin.velocity);

        if (ran_back_already)
        {
            Vector3 destination = new Vector3(1.8f, 0.0f, 8.5f);
            //float time = Time.deltaTime * 5.0f;
            //ResetAnimations();
            timer += Time.deltaTime;
            animator.Play("run_back");
            animator.SetBool("run_back_bool", true);
            penguin.transform.position = Vector3.Lerp(penguin.position, destination, 0.075f);

            //if this is entered it means the penguin is already on it's way backward
            //if (!(penguin.position.z >= 5.0f))
            //{
            //    animator.SetBool("run_back_bool", true);
            //    Debug.Log("coroutine for run back!!");
            //    StartCoroutine(DelayAfterRunBack());
            //}
        }
        else
        {
            if (penguin.velocity.y == 0.0f)
            {
                Debug.Log(penguin.name + " y velocity friggin died");
                if (direction == "left")
                {
                    //walking left
                    animator.SetBool("idle_left_bool", true);
                    time_on_ground++;
                }
                else if (direction == "right")
                {
                    //moving right
                    animator.SetBool("idle_right_bool", true);
                    time_on_ground++;
                }

                Debug.Log(penguin.name + " tog: " + time_on_ground);
                if (time_on_ground > 130)
                {
                    time_on_ground = 0;
                    if (direction == "left")
                    {
                        StartCoroutine(DelayJumpAfterRun());
                    }
                    else if (direction == "right")
                    {
                        StartCoroutine(DelayJumpAfterRun());
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
                    direction = "left";
                    ResetAnimations();
                    animator.SetBool("walk_left_bool", true);
                }
                else
                {
                    //moving right
                    direction = "right";
                    ResetAnimations();
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
    }

    /*
     * This function allows the penguin to jump after performing a slide (run animation)
     */
    public IEnumerator DelayJumpAfterRun()
    {
        if (direction == "left")
        {
            animator.Play("run_left");
            Bounce(-900.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.05f);
            ResetAnimations();
            animator.SetBool("walk_left_bool", true);
        }
        else
        {
            animator.Play("run_right");
            Bounce(900.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.05f);
            ResetAnimations();
            animator.SetBool("walk_right_bool", true);
        }

        Bounce(0.0f, 850.0f, 0.0f);
    }

    /*
     * this method is called whenever a penguin collides with an object
     */
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Exhibit_Entrance")
        {
            if (has_collided_with_player && !is_in_pen)
            {
                //allows the penguin to enter the snow area because the collision with the invisible wall will be ignored
                Physics.IgnoreCollision(collision.collider, this.penguin_collider, true);
            }
            else
            {
                Bounce(-300.0f, 4.0f, 0.0f); //FIXME: change nums 
            }
        }

        if(collision.gameObject.name == "Wall_Left")
        {
            penguin.transform.position = new Vector3(transform.position.x + 2.0f, transform.position.y, -3.0f);
            Bounce(300.0f, 4.0f, 0.0f); //FIXME: change nums 
        }

        //if the penguin collides with the player
        if (collision.gameObject.name == "Player")
        {
            has_collided_with_player = true; //sets flag and allows penguin to enter henhouse
            Bounce(500.0f, 5.0f, 0.0f); //bounces the penguin with a substantial amount of force when colliding with the user
        }

        if (collision.gameObject.tag == "Fence_Tag")
        {
            Bounce(800.0f, 0.0f, 0.0f);
        }

        //if the penguin collides with the igloo_entrance
        if (collision.gameObject.name == "Igloo_Hole")
        {
            //destroys the penguin
            Destroy(this.gameObject);
            //decrements the number of penguins in scene
            game_runner_script.Decrement_Num_Penguins();
        }

        if (collision.gameObject.name == "Snow_Back")
        {
            var z_pos = penguin.position.z;
            var x_pos = penguin.position.x;
            if ((z_pos >= 7.5 && z_pos <= 9.2) && x_pos >= 3.0)
            {
                ResetAnimations();
                animator.Play("run_left");
                animator.SetBool("run_left_bool", true);
            }

            float rand_y; //the new value for the y of the penguin
            rand_y = Random.Range(35.0f, 65.0f);

            //50% of the time, the penguin will bounce to the right
            if (RandomNum() <= 5)
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

        if (collision.gameObject.name == "Snow_Front")
        {
            is_in_pen = true;

            if (!ran_back_already)
            {
                //sometimes it bounces back, sometimes it bounces left
                int rand_snow = RandomNum();

                if ((rand_snow <= 3 || game_runner_script.Get_Num_Penguins() < 2) && penguin.position.x >= 2.5f)
                {
                    ran_back_already = true;
                }
                else
                {
                    ResetAnimations();
                    penguin.transform.position = new Vector3(transform.position.x, transform.position.y, -3.0f);
                    animator.Play("run_left");
                    animator.SetBool("run_left_bool", true);
                    Bounce(-2000.0f, 5.0f, 0.0f);
                }
            }
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
            is_in_pen = false;

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
                if (rand <= 5 || cur_num_penguins <= 4)
                {
                    //switch to track two
                    penguin.transform.position = new Vector3(cur_x, cur_y, -5.0f);
                }
            }

            float rand_y; //the new value for the y of the penguin
            rand_y = Random.Range(35.0f, 65.0f);

            //50% of the time, the penguin will bounce to the right
            if (RandomNum() <= 5)
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
    }

    public IEnumerator DelayAfterRunBack()
    {
        penguin.constraints = RigidbodyConstraints.None;
        penguin.constraints = RigidbodyConstraints.FreezeRotation;
        animator.Play("run_back");
        Bounce(0.0f, 0.0f, 1000.0f);
        yield return new WaitForSeconds(0.5f);
        ResetAnimations();
        animator.SetBool("walk_back_bool", true);
        animator.Play("walk_back");
        Bounce(0.0f, 2000.0f, 0.0f);
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
        animator.SetBool("walk_back_bool", false);
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
