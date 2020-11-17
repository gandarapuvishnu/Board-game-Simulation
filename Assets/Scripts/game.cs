using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

public class game : MonoBehaviour
{
    //variables
    public GameObject[] blocks;
    public GameObject[] scoreboard;
    
    private Image brick;
    private int chance = -1;
    private int[] pos = { 0, 0, 0 };
    private int j = 0;
    private bool dot = false;
    private bool show = true;

    private bool make_boards_white = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Let the game begin.");
        // Initiate naming
        for (int i = 0; i < blocks.Length; i++)
        {
            Text txt = blocks[i].GetComponentInChildren<Text>();
            txt.fontSize = 150;
            RectTransform size = txt.GetComponent<RectTransform>();

            size.localPosition = new Vector3(0.0f, 2.8f, 0.0f);
            size.sizeDelta = new Vector2(41.0f, 20.0f);

            int cost = set_price(blocks[i].name);

            string costTxt = cost.ToString();

            if (blocks[i].name != "Start" && blocks[i].name != "Jail" && blocks[i].name != "Camp" && blocks[i].name != "Hotel"
                && blocks[i].name != "CHANCE" && blocks[i].name != "CHANGE" && blocks[i].name != "TAX"
                && blocks[i].name != "Bank(0)" && blocks[i].name != "Electricity"
                && blocks[i].name != "Railway Bill" && blocks[i].name != "Phone Bill")
            {
                //Debug.Log(blocks[i].name);
                txt.text = blocks[i].name;
                txt.text += "\n";
                txt.text += "\n";
                txt.text += costTxt;
            }
            else
            {
                //Debug.Log(blocks[i].name);
                txt.text = blocks[i].name;
            }

            size.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
        }

        // Initialize scoreboard

        for (int i = 1; i < 4; i++)
        {
            GameObject.Find("Score (" + i.ToString() + ")").GetComponent<Image>().color = Color.green;
        }

        for (int i = 1; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string obj_name = "Display_" + i.ToString() + "_score_" + j.ToString();
                GameObject temp = GameObject.Find(obj_name);

                if (i == j)
                    temp.GetComponent<Text>().text = "1500";
                else
                    temp.GetComponent<Text>().text = "0";
            }
        }
    }
    int generate_random_number(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    void play()
    {
        j = generate_random_number(2, 12);

        show_dice(j);

        //Update pos of player
        j = pos[chance] + j;

        //Check if the player crossed start
        if (j >= blocks.Length)
        {
            get_token();
            Debug.Log("Token collected by player "+ chance.ToString());
        }

        j %= blocks.Length;

        //Set current visited cell to green.
        set_cell_to_green(j);

        pos[chance] = j;

        Debug.Log("1:" + blocks[pos[0]].name + " 2:"+blocks[pos[1]].name + " 3:" + blocks[pos[2]].name);

        //Check whether the player can buy it
        if (blocks[j].name != "Start" && blocks[j].name != "Jail" && blocks[j].name != "Camp"
                && blocks[j].name != "Hotel"
                && blocks[j].name != "CHANCE" && blocks[j].name != "CHANGE" && blocks[j].name != "TAX"
                && blocks[j].name != "Bank(0)" && blocks[j].name != "Electricity"
                && blocks[j].name != "Railway Bill" && blocks[j].name != "Phone Bill")
        {
            string curr_cell = "Display_" + (chance + 1).ToString() + "_score_" + (chance + 1).ToString();
            int cost = get_cost(blocks[j].name);
            int budget = Int32.Parse(GameObject.Find(curr_cell).GetComponent<Text>().text);

            string not_owned = blocks[j].name + "\n\n" + cost.ToString();

            if (budget >= cost)
            {
                int rent = set_rent(blocks[j].name);

                if (not_owned == blocks[j].GetComponentInChildren<Text>().text)
                {
                    budget -= cost;
                    Debug.Log("Player " + (chance + 1).ToString() + " bought " + blocks[j].name + " with a price of "+cost);                    

                    string replacing_owner = blocks[j].name;

                    replacing_owner += "\nPlayer " + (chance + 1).ToString() + "\n" + rent.ToString();

                    blocks[j].GetComponentInChildren<Text>().text = replacing_owner;

                    GameObject.Find(curr_cell).GetComponent<Text>().text = budget.ToString();
                }
                else
                {
                    if((chance == 0 &&
                        blocks[j].GetComponentInChildren<Text>().text ==
                        blocks[j].name + "\nPlayer 1\n" + rent.ToString()) || 
                        (chance == 1 && blocks[j].GetComponentInChildren<Text>().text ==
                        blocks[j].name + "\nPlayer 2\n" + rent.ToString()) ||
                         (chance == 2 && blocks[j].GetComponentInChildren<Text>().text ==
                        blocks[j].name + "\nPlayer 3\n" + rent.ToString())
                        )
                    {
                        rent *= 2;
                        string replacing_rent = blocks[j].name;

                        replacing_rent += "\nPlayer " + (chance + 1).ToString() + "\n" + rent.ToString();

                        blocks[j].GetComponentInChildren<Text>().text = replacing_rent;
                        Debug.Log("Rent doubled.");
                    }
                    else
                    {
                        string owner = "";
                        string to_update_rent = "";
                        if (blocks[j].GetComponentInChildren<Text>().text == blocks[j].name + "\nPlayer 1\n" + rent.ToString())
                        { owner = "Player 1"; ; to_update_rent = "Display_1_score_1"; }
                        if (blocks[j].GetComponentInChildren<Text>().text == blocks[j].name + "\nPlayer 2\n" + rent.ToString())
                        { owner = "Player 2"; ; to_update_rent = "Display_2_score_2";}
                        else if (blocks[j].GetComponentInChildren<Text>().text == blocks[j].name + "\nPlayer 3\n" + rent.ToString())
                        { owner = "Player 3"; to_update_rent = "Display_3_score_3"; }

                        GameObject.Find(curr_cell).GetComponent<Text>().text = (Int32.Parse(GameObject.Find(curr_cell).GetComponent<Text>().text) - rent).ToString();
                        GameObject.Find(to_update_rent).GetComponent<Text>().text = (Int32.Parse(GameObject.Find(to_update_rent).GetComponent<Text>().text) + rent).ToString();

                        Debug.Log("Rent of " + rent.ToString() +" paid to " + owner + " by player " + (chance + 1).ToString());
                    }

                }
            }
        else if (budget < cost)
            {
                Debug.Log("Property cannot be owned due to low budget.");
            }
        }
        else
        {
            Debug.Log("Special place.");
        }
    }

    void set_cell_to_green(int curr_index)
    {
        //Can add moving animation for players.
        
        brick = blocks[curr_index].GetComponent<Image>();
        brick.color = Color.green;
    }

    void show_dice(int number)
    {
        string curr_dicer = "dice_" + (chance + 1).ToString();
        GameObject.Find(curr_dicer).GetComponent<Text>().text = number.ToString();
    }

    void get_token()
    {
        for (int j = 0; j < 4; j++)
        {
            string obj_name = "Display_" + (chance + 1).ToString() + "_score_" + j.ToString();
            GameObject temp = GameObject.Find(obj_name);

            if ((chance + 1) == j)
            {
                int t = Int32.Parse(temp.GetComponent<Text>().text);

                t += 1500;

                temp.GetComponent<Text>().text = t.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dot == true)
        {
            play();

            dot = false;
            show = true;
        }
        else
        {
            if (show ==  true)
            {
                Debug.Log("Press space to continue");
                show = false;
            }
            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                turn_scoreboard_white(make_boards_white);

                if (chance > -1)
                    scoreboard[chance].GetComponent<Image>().color = Color.white;

                //Increment chance
                chance = (chance + 1) % scoreboard.Length;
                //Update scoreboard color
                scoreboard[chance].GetComponent<Image>().color = Color.green;

                reset_original_color(j);

                dot = true;
            }
        }
    }

    void reset_original_color(int index)
    {
        string name = blocks[index].name;

        if ((name == "Start") || (name == "Jail") || (name == "Camp") || (name == "Hotel"))
            blocks[index].GetComponent<Image>().color = Color.yellow;

        else if (name == "CHANCE")
            blocks[index].GetComponent<Image>().color = Color.yellow;
        else if (name == "CHANGE")
            blocks[index].GetComponent<Image>().color = Color.magenta;

        else
            blocks[index].GetComponent<Image>().color = Color.red;
    }

    void turn_scoreboard_white(bool make_boards_white)
    {
        if (make_boards_white == false)
        {
            Debug.Log("Turning all scoreboards to white.");
            for (int i = 1; i < 4; i++)
            {
                GameObject.Find("Score (" + i.ToString() + ")").GetComponent<Image>().color = Color.white;
            }
            make_boards_white = true;
        }
    }

    int get_cost(string name)
    {
        if (name == "Shillong")
            return 5000;
        else if (name == "Patna")
            return 1500;
        else if (name == "Calicut")
            return 2000;
        else if (name == "Hyderabad")
            return 8500;
        else if (name == "Kutch")
            return 5000;
        else if (name == "Panaji")
            return 2200;

        else if (name == "Ranikhet")
            return 2000;

        else if (name == "Jodhpur")
            return 2500;
        else if (name == "Mussorie")
            return 3000;
        else if (name == "Kolkata")
            return 6500;

        else if (name == "Srinagar")
            return 5000;
        else if (name == "Jaipur")
            return 3000;

        else if (name == "Rajkot")
            return 4000;
        else if (name == "Chennai")
            return 7000;

        else if (name == "Indore")
            return 3500;

        else if (name == "Lucknow")
            return 4000;

        else if (name == "New Delhi")
            return 4000;
        else if (name == "Darjeeling")
            return 2500;

        else if (name == "Mysore")
            return 2500;

        else if (name == "Bangalore")
            return 6000;
        else if (name == "Dalhousie")
            return 3300;
        else if (name == "Bhopal")
            return 3500;
        else if (name == "Chandigarh")
            return 2500;
        else if (name == "Kanpur")
            return 4500;

        else
            return 65461654;
    }

    int set_rent(string name)
    {
        if (name == "Shillong")
            return 550;
        else if (name == "Patna")
            return 200;
        else if (name == "Calicut")
            return 150;
        else if (name == "Hyderabad")
            return 1200;
        else if (name == "Kutch")
            return 400;
        else if (name == "Panaji")
            return 200;

        else if (name == "Ranikhet")
            return 150;

        else if (name == "Jodhpur")
            return 200;
        else if (name == "Mussorie")
            return 300;
        else if (name == "Kolkata")
            return 800;

        else if (name == "Srinagar")
            return 400;
        else if (name == "Jaipur")
            return 250;

        else if (name == "Rajkot")
            return 400;
        else if (name == "Chennai")
            return 900;

        else if (name == "Indore")
            return 300;

        else if (name == "Lucknow")
            return 400;

        else if (name == "New Delhi")
            return 400;
        else if (name == "Darjeeling")
            return 200;

        else if (name == "Mysore")
            return 200;

        else if (name == "Bangalore")
            return 750;
        else if (name == "Dalhousie")
            return 300;
        else if (name == "Bhopal")
            return 300;
        else if (name == "Chandigarh")
            return 200;
        else
            return 100;
    }

    int set_price(string name)
    {
        if (name == "Shillong")
            return 5000;
        else if (name == "Patna")
            return 1500;
        else if (name == "Calicut")
            return 2000;
        else if (name == "Hyderabad")
            return 8500;
        else if (name == "Kutch")
            return 5000;
        else if (name == "Panaji")
            return 2200;

        else if (name == "Ranikhet")
            return 2000;

        else if (name == "Jodhpur")
            return 2500;
        else if (name == "Mussorie")
            return 3000;
        else if (name == "Kolkata")
            return 6500;

        else if (name == "Srinagar")
            return 5000;
        else if (name == "Jaipur")
            return 3000;

        else if (name == "Rajkot")
            return 4000;
        else if (name == "Chennai")
            return 7000;

        else if (name == "Indore")
            return 3500;

        else if (name == "Lucknow")
            return 4000;

        else if (name == "New Delhi")
            return 4000;
        else if (name == "Darjeeling")
            return 2500;

        else if (name == "Mysore")
            return 2500;

        else if (name == "Bangalore")
            return 6000;
        else if (name == "Dalhousie")
            return 3300;
        else if (name == "Bhopal")
            return 3500;
        else if (name == "Chandigarh")
            return 2500;
        else
            return 4000;
    }
}
