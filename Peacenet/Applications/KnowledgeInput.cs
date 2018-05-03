using System;
using Plex.Engine.GUI;
using Plex.Engine;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Plex.Engine.Saves;
using Plex.Engine.Themes;

namespace Peacenet
{
    [AppLauncher("Knowledge Input", "Accessories", "A game that rewards Code Points when you list enough words within a category such as fruits")]
    [SpecialEventAppLauncher(1, 4)]
    public class KnowledgeInput : Window
    {
        ListView ListBox1 = new ListView();
        Panel pnlintro = new Panel();
        Panel pnlcategorydisplay = new Panel();
        Label lblnextreward = new Label();
        TextBox guessbox = new TextBox();
        Label lblcurrentlevel = new Label();
        Label Label5 = new Label();
        ListView listblistedstuff = new ListView();
        Label lbltillnextlevel = new Label();
        Label lbltotal = new Label();
        Button btnstart = new Button();
        Label lblcatedescription = new Label();
        Label lblcategory = new Label();
        Label Label4 = new Label();
        Label Label3 = new Label();
        Label Label2 = new Label();
        Label Label1 = new Label();
        Label Label6 = new Label();

        bool guessalreadydone;
        bool guesscorrect;
        bool levelup;
        int rewardbase;

        string[] animalslist;
        string[] fruitslist;
        string[] countrieslist;
        string[] carbrandslist;
        string[] gameconsoleslist;
        string[] elementslist;

        int totalguessed;
        int level;
        int tillnextlevel;

        int decider;

        public KnowledgeInput(WindowSystem _winsys) : base(_winsys)
        {
            ListBox1.X = 9;
            ListBox1.Y = 49;
            ListBox1.Width = 175;
            ListBox1.Height = 216;
            ListBox1.Layout = ListViewLayout.List;

            pnlintro.AddChild(pnlcategorydisplay);
            pnlintro.AddChild(Label4);
            pnlintro.AddChild(Label3);
            pnlintro.AddChild(Label2);
            pnlintro.AddChild(Label1);
            pnlintro.X = 191;
            pnlintro.Y = 0;
            pnlintro.Width = 478;
            pnlintro.Height = 272;

            pnlcategorydisplay.AddChild(lblnextreward);
            pnlcategorydisplay.AddChild(guessbox);
            pnlcategorydisplay.AddChild(lblcurrentlevel);
            pnlcategorydisplay.AddChild(Label5);
            pnlcategorydisplay.AddChild(listblistedstuff);
            pnlcategorydisplay.AddChild(lbltillnextlevel);
            pnlcategorydisplay.AddChild(lbltotal);
            pnlcategorydisplay.AddChild(btnstart);
            pnlcategorydisplay.AddChild(lblcatedescription);
            pnlcategorydisplay.AddChild(lblcategory);
            pnlcategorydisplay.X = 0;
            pnlcategorydisplay.Y = 0;
            pnlcategorydisplay.Width = 478;
            pnlcategorydisplay.Height = 272;

            lblnextreward.AutoSize = true;
            lblnextreward.X = 36;
            lblnextreward.Y = 110;
            lblnextreward.Width = 244;
            lblnextreward.Height = 20;
            lblnextreward.Text = "Reward for completing level 1: 5CP";

            guessbox.X = 11;
            guessbox.Y = 147;
            guessbox.Width = 297;
            guessbox.Height = 45;
            guessbox.Text = "Enter Guess Here";

            lblcurrentlevel.X = -6;
            lblcurrentlevel.Y = 77;
            lblcurrentlevel.Width = 331;
            lblcurrentlevel.Height = 42;
            lblcurrentlevel.Text = "Current Level: 1";

            Label5.AutoSize = true;
            Label5.X = 340;
            Label5.Y = 12;
            Label5.Width = 123;
            Label5.Height = 18;
            Label5.Text = "All Ready Done";

            listblistedstuff.X = 340;
            listblistedstuff.Y = 41;
            listblistedstuff.Width = 129;
            listblistedstuff.Height = 221;
            listblistedstuff.Layout = ListViewLayout.List;

            lbltillnextlevel.AutoSize = true;
            lbltillnextlevel.X = 8;
            lbltillnextlevel.Y = 250;
            lbltillnextlevel.Width = 146;
            lbltillnextlevel.Height = 16;
            lbltillnextlevel.Text = "Words Until Next Level:";

            lbltotal.AutoSize = true;
            lbltotal.X = 191;
            lbltotal.Y = 250;
            lbltotal.Width = 66;
            lbltotal.Height = 16;
            lbltotal.Text = "Guessed:";

            btnstart.X = 11;
            btnstart.Y = 198;
            btnstart.Width = 297;
            btnstart.Height = 46;
            btnstart.Text = "Submit Word";

            lblcatedescription.X = 11;
            lblcatedescription.Y = 48;
            lblcatedescription.Width = 297;
            lblcatedescription.Height = 26;
            lblcatedescription.Text = "There are many animals out there! Can you list them all? \nNote that this is a list of common animals, not every animal!";

            lblcategory.X = 11;
            lblcategory.Y = 8;
            lblcategory.Width = 297;
            lblcategory.Height = 39;
            lblcategory.Text = "Animals";

            Label4.AutoSize = true;
            Label4.X = 52;
            Label4.Y = 235;
            Label4.Width = 382;
            Label4.Height = 20;
            Label4.Text = "Select A Category On the Left To Start Playing";

            Label3.AutoSize = true;
            Label3.X = 187;
            Label3.Y = 72;
            Label3.Width = 112;
            Label3.Height = 20;
            Label3.Text = "How To Play:";

            Label2.AutoSize = true;
            Label2.X = 61;
            Label2.Y = 97;
            Label2.Width = 354;
            Label2.Height = 96;
            Label2.Text = "Your goal in this game is to list as many words as possible\nfrom a certain category of objects such as \"Animals\".\n\nYou start off on level 1 and your goal is to list 10 words to\nreach the next level. Upon reaching each new level you will\nbe rewarded with an increasing number of code points.";

            Label1.AutoSize = true;
            Label1.X = 75;
            Label1.Y = 12;
            Label1.Width = 316;
            Label1.Height = 25;
            Label1.Text = "Welcome to Knowledge Input";

            Label6.AutoSize = true;
            Label6.X = 30;
            Label6.Y = 8;
            Label6.Width = 131;
            Label6.Height = 29;
            Label6.Text = "Categories";

            AddChild(pnlintro);
            AddChild(Label6);
            AddChild(ListBox1);

            Width = 669;
            Height = 272;
            Title = "Knowledge Input";

            pnlcategorydisplay.Visible = false;
            level = 1;
            tillnextlevel = 10;
            makeanimallist();
            makefruitlist();
            makecountrieslist();
            makecarbrandslist();
            makegameconsoleslist();
            makeelementslist();

            ListBox1.AddItem(new ListViewItem { Value = "Animals" });
            ListBox1.AddItem(new ListViewItem { Value = "Fruits" });
            ListBox1.AddItem(new ListViewItem { Value = "Countries" });
            ListBox1.AddItem(new ListViewItem { Value = "Car Brands" });
            ListBox1.AddItem(new ListViewItem { Value = "Game Consoles" });
            ListBox1.AddItem(new ListViewItem { Value = "Elements" });
            ListBox1.ItemClicked += ListBox1_SelectedIndexChanged;

            btnstart.Click += btnstart_Click;
            guessbox.Click += guessbox_click;
            guessbox.KeyEvent += guessbox_keydown;
        }

        private void ListBox1_SelectedIndexChanged(ListViewItem item)
        {
            switch (item.Value)
            {
                case "Animals":
                    loadsavepoint("Animals", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Animals.lst", "There are many animals out there! Can you list them all?" + Environment.NewLine + "Note that you get points for listing animals... not animal breeds!", animalslist);
                    break;
                case "Fruits":
                    loadsavepoint("Fruits", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Fruits.lst", "Do you get your daily serving of fruit each day?" + Environment.NewLine + "Really...? See if you can list them then ;)", fruitslist);
                    break;
                case "Countries":
                    loadsavepoint("Countries", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Countries.lst", "Ever wanted to travel the entire world?" + Environment.NewLine + "Well before you do see if you can list every country in the world!", countrieslist);
                    break;
                case "Car Brands":
                    loadsavepoint("Car Brands", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Car Brands.lst", "Can you list every single car brand?" + Environment.NewLine + "Don't use words like automobiles, motors or cars!", carbrandslist);
                    break;
                case "Game Consoles":
                    loadsavepoint("Game Consoles", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Game Consoles.lst", "Do you call yourself a gamer?" + Environment.NewLine + "Earn that title by listing non-handheld game consoles!", gameconsoleslist);
                    break;
                case "Elements":
                    loadsavepoint("Elements", 10, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Elements.lst", "Have you memorized the periodic table of elements?" + Environment.NewLine + "No? Well don't even attempt trying to guess them all here!", elementslist);
                    break;
            }
        }

        private void handleword()
        {
            switch (ListBox1.SelectedItem.Value)
            {
                case "Animals":
                    handlewordtype(animalslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Animals.lst");
                    break;
                case "Fruits":
                    handlewordtype(fruitslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Fruits.lst");
                    break;
                case "Countries":
                    handlewordtype(countrieslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Countries.lst");
                    break;
                case "Car Brands":
                    handlewordtype(carbrandslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Car Brands.lst");
                    break;
                case "Game Consoles":
                    handlewordtype(gameconsoleslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Game Consoles.lst");
                    break;
                case "Elements":
                    handlewordtype(elementslist, "C:\\ShiftOS\\SoftwareData\\KnowledgeInput\\Elements.lst");
                    break;
            }

            guessbox.Text = "";
            listblistedstuff.SelectedIndex = listblistedstuff.Items.Length - 1;
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            handleword();
        }

        private void guessbox_click(object sender, EventArgs e)
        {
            guessbox.Text = "";
        }

        private void guessbox_keydown(object sender, MonoGame.Extended.Input.InputListeners.KeyboardEventArgs e)
        {
            if (e.Key == Keys.Enter)
                handleword();
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime time)
        {
            base.OnUpdate(time);
            if (decider > 0)
            {
                decider -= (int)Math.Round(time.ElapsedGameTime.TotalMilliseconds);
                if (decider <= 0)
                {
                    lblcurrentlevel.Text = $"Current Level: {level}";
                    lblnextreward.Text = $"Reward for completing level {level} : {rewardbase * level} CP";
                    guessalreadydone = false;
                    guesscorrect = false;
                    levelup = false;
                }
            }
        }

        [Dependency]
        SaveManager save;

        private void loadsavepoint(string title, int reward, string loadpath, string info, string[] listtype)
        {
            lblcategory.Text = title;
            rewardbase = reward;
            listblistedstuff.ClearItems();
            foreach (var item in save.GetValue(loadpath, new string[0]))
                listblistedstuff.AddItem(new ListViewItem { Value = item });
            totalguessed = listblistedstuff.Items.Length;
            level = (int)Math.Ceiling((totalguessed / 10.0));
            tillnextlevel = Math.Abs(totalguessed - (level * 10));
            if (tillnextlevel == 0)
            {
                level = level + 1;
                tillnextlevel = 10;
            }

            lblcatedescription.Text = info;
            Label4.Visible = false;
            Label3.Visible = false;
            Label2.Visible = false;
            Label1.Visible = false;
            pnlcategorydisplay.Visible = true;
            lbltillnextlevel.Text = "Words Until Next Level: " + tillnextlevel;
            lblcurrentlevel.Text = "Current Level: " + level;
            lbltotal.Text = "Guessed: " + totalguessed + "/" + listtype.Length;
            lblnextreward.Text = "Reward for completing level " + level + " : " + rewardbase * level + "CP";
        }

        private void handlewordtype(string[] listtype, string savepath)
        {
            string userguess = guessbox.Text;
            userguess = userguess.ToLower();
            foreach (string Str in listtype)
            {
                if (Str == userguess)
                {
                    if (listblistedstuff.Items.Any(i => i.Value == userguess))
                    {
                        guessalreadydone = true;
                    }
                    else
                    {
                        guesscorrect = true;
                        listblistedstuff.AddItem(new ListViewItem { Value = userguess });
                        tillnextlevel = tillnextlevel - 1;
                        totalguessed = totalguessed + 1;
                        save.SetValue(savepath, listblistedstuff.Items.Select(i => i.Value).ToArray());
                        if (tillnextlevel == 0)
                        {
                            levelup = true;
                            tillnextlevel = 10;
                            level = level + 1;
                        }
                    }
                }
            }

            lbltillnextlevel.Text = "Words Until Next Level: " + tillnextlevel;
            lblcurrentlevel.Text = "Current Level: " + level;
            lbltotal.Text = "Guessed: " + totalguessed + "/" + listtype.Length;
            lblnextreward.Text = "Reward for completing level " + level + " : " + rewardbase * level + "CP";
            if (levelup == true)
            {
                lblcurrentlevel.Text = "Level Up!";
                lblnextreward.Text = "You have earned " + rewardbase * (level - 1) + " Code Points!";
                decider = 2000;
            }
            else
            {
                if (guessalreadydone == true)
                {
                    lblcurrentlevel.Text = "Already Guessed";
                    decider = 500;
                }
                else
                {
                    if (guesscorrect == true)
                    {
                        lblcurrentlevel.Text = "Correct :)";
                        decider = 500;
                    }
                    else
                    {
                        lblcurrentlevel.Text = "Wrong :(";
                        decider = 500;
                    }
                }
            }
        }

        private void makeanimallist()
        {
            animalslist = new[]
            { 
                "aardvark",
                "albatross",
                "alligator",
                "alpaca",
                "ant",
                "anteater",
                "antelope",
                "ape",
                "armadillo",
                "ass",
                "baboon",
                "badger",
                "barracuda",
                "bat",
                "bear",
                "beaver",
                "bee",
                "bison",
                "boar",
                "buffalo",
                "butterfly",
                "camel",
                "caribou",
                "cat",
                "caterpillar",
                "cow",
                "chamois",
                "cheetah",
                "chicken",
                "chimpanzee",
                "chinchilla",
                "chough",
                "clam",
                "cobra",
                "cockroach",
                "cod",
                "cormorant",
                "coyote",
                "crab",
                "crane",
                "crocodile",
                "crow",
                "curlew",
                "deer",
                "dinosaur",
                "dog",
                "dogfish",
                "dolphin",
                "donkey",
                "dotterel",
                "dove",
                "dragonfly",
                "duck",
                "dugong",
                "dunlin",
                "eagle",
                "echidna",
                "eel",
                "eland",
                "elephant",
                "elephant seal",
                "elk",
                "emu",
                "falcon",
                "ferret",
                "finch",
                "fish",
                "flamingo",
                "fly",
                "fox",
                "frog",
                "galago",
                "gaur",
                "gazelle",
                "gerbil",
                "giant panda",
                "giraffe",
                "gnat",
                "gnu",
                "goat",
                "goldfinch",
                "goldfish",
                "goose",
                "gorilla",
                "goshawk",
                "grasshopper",
                "grouse",
                "guanaco",
                "guineafowl",
                "guinea pig",
                "gull",
                "hamster",
                "hare",
                "hawk",
                "hedgehog",
                "heron",
                "herring",
                "hippopotamus",
                "hornet",
                "horse",
                "human",
                "humming bird",
                "hyena",
                "jackal",
                "jaguar",
                "jay",
                "jellyfish",
                "kangaroo",
                "koala",
                "komodo dragon",
                "kouprey",
                "kudu",
                "lizard",
                "lark",
                "lemur",
                "leopard",
                "lion",
                "llama",
                "lobster",
                "locust",
                "loris",
                "louse",
                "lyrebird",
                "magpie",
                "mallard",
                "manatee",
                "marten",
                "meerkat",
                "mink",
                "mole",
                "monkey",
                "moose",
                "mosquito",
                "mouse",
                "mule",
                "narwhal",
                "newt",
                "nightingale",
                "octopus",
                "okapi",
                "opossum",
                "oryx",
                "ostrich",
                "otter",
                "owl",
                "ox",
                "oyster",
                "panther",
                "parrot",
                "partridge",
                "peafowl",
                "pelican",
                "penguin",
                "pheasant",
                "pig",
                "pigeon",
                "pony",
                "porcupine",
                "porpoise",
                "prairie dog",
                "quail",
                "quelea",
                "rabbit",
                "raccoon",
                "rail",
                "ram",
                "rat",
                "raven",
                "red deer",
                "red panda",
                "reindeer",
                "rhinoceros",
                "rook",
                "ruff",
                "salamander",
                "salmon",
                "sand dollar",
                "sandpiper",
                "sardine",
                "scorpion",
                "sea lion",
                "sea urchin",
                "seahorse",
                "seal",
                "shark",
                "sheep",
                "shrew",
                "shrimp",
                "skunk",
                "snail",
                "snake",
                "spider",
                "squid",
                "squirrel",
                "starling",
                "stingray",
                "stink bug",
                "stork",
                "swallow",
                "swan",
                "tapir",
                "tarsier",
                "termite",
                "tiger",
                "toad",
                "trout",
                "turkey",
                "turtle",
                "vicuña",
                "viper",
                "vulture",
                "wallaby",
                "walrus",
                "wasp",
                "water buffalo",
                "weasel",
                "whale",
                "wolf",
                "wolverine",
                "wombat",
                "woodcock",
                "woodpecker",
                "worm",
                "wren",
                "yak",
                "zebra",
                "bird"
            };
        }

        private void makefruitlist()
        {
            fruitslist = new[]
            {
                "apple",
                "apricot",
                "avocado",
                "banana",
                "breadfruit",
                "bilberry",
                "blackberry",
                "blackcurrant",
                "blueberry",
                "boysenberry",
                "cantaloupe",
                "currant",
                "cherry",
                "cherimoya",
                "chili",
                "cloudberry",
                "coconut",
                "damson",
                "date",
                "dragonfruit",
                "durian",
                "elderberry",
                "feijoa",
                "fig",
                "gooseberry",
                "grape",
                "grapefruit",
                "guava",
                "huckleberry",
                "honeydew",
                "jackfruit",
                "jambul",
                "jujube",
                "kiwi fruit",
                "kumquat",
                "legume",
                "lemon",
                "lime",
                "loquat",
                "lychee",
                "mango",
                "melon",
                "canary melon",
                "cantaloupe",
                "honeydew",
                "watermelon",
                "rock melon",
                "nectarine",
                "nut",
                "orange",
                "clementine",
                "mandarine",
                "tangerine",
                "papaya",
                "passionfruit",
                "peach",
                "bell pepper",
                "pear",
                "persimmon",
                "physalis",
                "plum",
                "pineapple",
                "pomegranate",
                "pomelo",
                "purple mangosteen",
                "quince",
                "raspberry",
                "rambutan",
                "redcurrant",
                "salal berry",
                "satsuma",
                "star fruit",
                "strawberry",
                "tamarillo",
                "tomato",
                "ugli fruit"
            };
        }

        private void makecountrieslist()
        {
            countrieslist = new[]
            {
                "afghanistan",
                "albania",
                "algeria",
                "american samoa",
                "andorra",
                "angola",
                "anguilla",
                "antigua",
                "argentina",
                "armenia",
                "aruba",
                "australia",
                "austria",
                "azerbaijan",
                "bahamas",
                "bahrain",
                "bangladesh",
                "barbados",
                "barbuda",
                "belarus",
                "belgium",
                "belize",
                "benin",
                "bermuda",
                "bhutan",
                "bolivia",
                "bosnia",
                "botswana",
                "bouvet island",
                "brazil",
                "brunei",
                "bulgaria",
                "burkina faso",
                "burundi",
                "cambodia",
                "cameroon",
                "canada",
                "cape verde",
                "cayman islands",
                "central african republic",
                "chad",
                "chile",
                "china",
                "christmas island",
                "cocos islands",
                "colombia",
                "comoros",
                "democratic republic of the congo",
                "republic of congo",
                "cook islands",
                "costa rica",
                "croatia",
                "cuba",
                "cyprus",
                "czech republic",
                "denmark",
                "djibouti",
                "dominica",
                "dominican republic",
                "ecuador",
                "egypt",
                "el salvador",
                "equatorial guinea",
                "eritrea",
                "estonia",
                "ethiopia",
                "falkland islands",
                "faroe islands",
                "fiji",
                "finland",
                "france",
                "french guiana",
                "gabon",
                "gambia",
                "georgia",
                "germany",
                "ghana",
                "gibraltar",
                "greece",
                "greenland",
                "grenada",
                "guadeloupe",
                "guam",
                "guatemala",
                "guinea",
                "guinea bissau",
                "guyana",
                "haiti",
                "holy see",
                "honduras",
                "hong kong",
                "hungary",
                "iceland",
                "india",
                "indonesia",
                "iran",
                "iraq",
                "ireland",
                "israel",
                "italy",
                "ivory coas",
                "jamaica",
                "japan",
                "jordan",
                "kazakhstan",
                "kenya",
                "kiribati",
                "kuwait",
                "kyrgyzstan",
                "laos",
                "latvia",
                "lebanon",
                "lesotho",
                "liberia",
                "libya",
                "liechtenstein",
                "lithuania",
                "luxembourg",
                "macau",
                "macedonia",
                "madagascar",
                "malawi",
                "malaysia",
                "maldives",
                "mali",
                "malta",
                "marshall islands",
                "martinique",
                "mauritania",
                "mauritius",
                "mayotte",
                "mexico",
                "micronesia",
                "moldova",
                "monaco",
                "mongolia",
                "montenegro",
                "montserrat",
                "morocco",
                "mozambique",
                "myanmar",
                "namibia",
                "nauru",
                "nepal",
                "netherlands",
                "new caledonia",
                "new zealand",
                "nicaragua",
                "niger",
                "nigeria",
                "niue",
                "norfolk island",
                "north korea",
                "northern mariana islands",
                "norway",
                "oman",
                "pakistan",
                "palau",
                "panama",
                "papua new guinea",
                "paraguay",
                "peru",
                "philippines",
                "pitcairn island",
                "poland",
                "polynesia",
                "portugal",
                "puerto rico",
                "qatar",
                "reunion",
                "romania",
                "russia",
                "rwanda",
                "saint helena",
                "saint kitts and nevis",
                "saint lucia",
                "saint pierre and miquelon",
                "saint vincent and grenadines",
                "samoa",
                "san marino",
                "sao tome and principe",
                "saudi arabia",
                "senegal",
                "serbia",
                "seychelles",
                "sierra leone",
                "singapore",
                "slovakia",
                "slovenia",
                "solomon islands",
                "somalia",
                "south africa",
                "south georgia and south sandwich islands",
                "south korea",
                "south sudan",
                "spain",
                "sri lanka",
                "sudan",
                "suriname",
                "svalbard and jan mayen islands",
                "swaziland",
                "sweden",
                "switzerland",
                "syria",
                "taiwan",
                "tajikistan",
                "tanzania",
                "thailand",
                "east timor",
                "togo",
                "tokelau",
                "tonga",
                "trinidad and tobago",
                "tunisia",
                "turkey",
                "turkmenistan",
                "turks and caicos islands",
                "tuvalu",
                "uganda",
                "ukraine",
                "united arab emirates",
                "united kingdom",
                "united states",
                "uruguay",
                "uzbekistan",
                "vanuatu",
                "venezuela",
                "vietnam",
                "virgin islands",
                "wallis and futuna islands",
                "yemen",
                "zambia",
                "zimbabwe"
            };
        }

        public void makecarbrandslist()
        {
            carbrandslist = new[]
            {
                "8 chinkara",
                "aba",
                "abarth",
                "ac",
                "ac schnitzer",
                "acura",
                "adam",
                "adams-farwell",
                "adler",
                "aero",
                "aga",
                "agrale",
                "aixam",
                "alfa romeo",
                "allard",
                "alpine",
                "alvis",
                "anadol",
                "anasagasti",
                "angkor",
                "apollo",
                "armstrong siddeley",
                "aro",
                "ascari",
                "ashok leyland",
                "aston martin",
                "auburn",
                "audi",
                "austin",
                "austin-healey",
                "auto-mixte",
                "autobianchi",
                "automobile dacia",
                "avia",
                "avtoframos",
                "awz",
                "bahman",
                "bajaj",
                "barkas",
                "bate",
                "bentley",
                "bharath benz",
                "bitter",
                "bmc",
                "bmw",
                "bollore",
                "borgward",
                "bricklin",
                "bristol",
                "british leyland",
                "bufori",
                "bugatti",
                "buick",
                "bussing",
                "c-fee",
                "cadillac",
                "callaway",
                "caterham",
                "cherdchai",
                "chevrolet",
                "chrysler",
                "citroen",
                "cizeta",
                "coda",
                "cord",
                "crespi",
                "crobus",
                "daf",
                "daihatsu",
                "daimler",
                "datsun",
                "davis",
                "dc design",
                "de tomaso",
                "delorean",
                "derby",
                "dina",
                "dkw",
                "dodge",
                "dok-ing",
                "dok-ing xd",
                "dome",
                "donkervoort",
                "dr",
                "duesenberg",
                "e-z-go",
                "eagle",
                "edsel",
                "eicher",
                "elfin",
                "elva",
                "enzmann",
                "essex",
                "esther",
                "exagon",
                "falcon",
                "fap",
                "ferrari",
                "fiat",
                "fisker",
                "force",
                "ford",
                "fpv",
                "gaz",
                "gengatharan",
                "geo",
                "ghandhara",
                "ghandhara nissan",
                "gillet",
                "ginetta",
                "gkd",
                "glas",
                "global electric",
                "gm daewoo",
                "gm uzbekistan",
                "gmc",
                "goliath",
                "gordon keeble",
                "graham-paige",
                "guleryuz karoseri",
                "gumpert",
                "gurgel",
                "hansa",
                "hattat",
                "heinkel",
                "hennessey",
                "hero",
                "hillman",
                "hindustan",
                "hino",
                "hinopak",
                "hispano-argentina",
                "holden",
                "hommell",
                "honda",
                "honda atlas",
                "horch",
                "hsv",
                "huet brothers",
                "humber",
                "hummer",
                "hupmobile",
                "hyundai",
                "iame",
                "icml",
                "ida-opel",
                "ika",
                "ikarbus",
                "ikco",
                "indus",
                "infiniti",
                "inokom",
                "intermeccanica",
                "international harvester",
                "isuzu",
                "isuzu anadolu",
                "italika",
                "izh",
                "jaguar cars",
                "jeep",
                "jensen",
                "josse",
                "jowett",
                "jv man",
                "kaipan",
                "kaiser",
                "karsan",
                "kerman",
                "kia",
                "kia",
                "kish khodro",
                "kissel",
                "koenigsegg",
                "lada",
                "laforza",
                "lamborghini",
                "lanchester",
                "lancia",
                "land rover",
                "lasalle",
                "lexus",
                "ligier",
                "lincoln",
                "lister",
                "lloyd",
                "lobini",
                "locomobile",
                "lotus",
                "mahindra",
                "man",
                "mansory",
                "marcos",
                "marmon",
                "marussia",
                "maruti suzuki",
                "maserati",
                "master",
                "mastretta",
                "matra",
                "maybach",
                "mazda",
                "mclaren",
                "mdi",
                "mercedes",
                "mercury",
                "micro",
                "microcar",
                "mini",
                "mini cooper",
                "mitsubishi",
                "mitsuoka",
                "morgan",
                "morris",
                "moskvitch",
                "mosler",
                "multicar",
                "mvm",
                "nag",
                "nagant",
                "nash",
                "navistar",
                "naza",
                "neobus",
                "neoplan",
                "nissan",
                "noble",
                "nsu",
                "oldsmobile",
                "oltcit",
                "opel",
                "orient",
                "otokar",
                "otosan",
                "oyak",
                "p.a.r.s moto",
                "packard",
                "pagani",
                "pak suzuki",
                "panoz",
                "pars khodro",
                "perodua",
                "peugeot",
                "pgo",
                "pieper",
                "pierce-arrow",
                "plymouth",
                "pontiac",
                "porsche",
                "praga",
                "premier",
                "proto",
                "proton",
                "puma",
                "ram",
                "ramirez",
                "regal",
                "renault",
                "renault samsung",
                "reo",
                "riley",
                "rimac",
                "robur",
                "rolls royce",
                "rover",
                "ruf",
                "russo-balt",
                "saab",
                "saipa",
                "saleen",
                "samavto",
                "saturn",
                "sbarro",
                "scania",
                "scion",
                "shane moto",
                "siam v.m.c.",
                "siata",
                "simson",
                "singer",
                "skoda",
                "sound",
                "spyker",
                "ssangyong",
                "standard",
                "stealth",
                "sterling",
                "studebaker",
                "subaru",
                "sunbeam",
                "suzuki",
                "tac",
                "tafe",
                "tata",
                "tatra",
                "td2000",
                "temsa",
                "tesla",
                "th!nk",
                "thai rung",
                "the jamie stahley car",
                "tickford",
                "toyota",
                "trabant",
                "tranvias-cimex",
                "triumph",
                "trojan",
                "troller",
                "tucker",
                "turk traktor",
                "tvr",
                "tvs",
                "uaz",
                "vam sa",
                "vauxhall",
                "venturi",
                "vignale",
                "volkswagen",
                "volvo",
                "wanderer",
                "wartburg",
                "wiesmann",
                "willys",
                "wolseley",
                "yamaha",
                "yo-mobile",
                "zastava",
                "zenvo",
                "zil",
                "zoragy"
            };
        }

        public void makegameconsoleslist()
        {
            gameconsoleslist = new[]
            {
                "magnavox odyssey",
                "ping-o-tronic",
                "telstar",
                "apf tv fun",
                "philips odyssey",
                "radio shack tv scoreboard",
                "binatone tv master mk iv",
                "color tv game 6",
                "color tv game 15",
                "color tv racing 112",
                "color tv game block breaker",
                "computer tv game",
                "bss 01",
                "fairchild channel f",
                "fairchild channel f system ii",
                "rca studio ii",
                "atari 2600",
                "atari 2600 jr",
                "atari 2800",
                "coleco gemini",
                "bally astrocade",
                "vc 4000",
                "magnavox odyssey 2",
                "apf imagination machine",
                "intellivision",
                "playcable",
                "bandai super vision 8000",
                "intellivision ii",
                "vtech creativision",
                "epoch cassette vision",
                "super cassette vision",
                "arcadia 2001",
                "atari 5200",
                "atari 5100",
                "colecovision",
                "entex adventure vision",
                "vectrex",
                "rdi halcyon",
                "pv-1000",
                "commodore 64 games system",
                "amstrad gx4000",
                "atari 7800",
                "atari xegs",
                "sega sg-1000",
                "sega master system",
                "nintendo entertainment system",
                "sharp nintendo television",
                "nes-101",
                "family computer disk system",
                "zemmix",
                "action max",
                "sega genesis",
                "sega pico",
                "pc engine",
                "konix multisystem",
                "neo-geo",
                "neo-geo cd",
                "neo-geo cdz",
                "commodore cdtv",
                "memorex vis",
                "super nintendo entertainment system",
                "sf-1 snes tv",
                "snes 2",
                "snes-cd",
                "satellaview",
                "cd-i",
                "turboduo",
                "super a'can",
                "pioneer laseractive",
                "fm towns marty",
                "apple bandai pippin",
                "pc-fx",
                "atari panther",
                "atari jaguar",
                "atari jaguar cd",
                "playstation",
                "net yaroze",
                "sega saturn",
                "3do interactive multiplayer",
                "amiga cd32",
                "casio loopy",
                "playdia",
                "nintendo 64",
                "nintendo 64dd",
                "sega neptune",
                "apextreme",
                "atari flashback",
                "atari jaguar ii",
                "dreamcast",
                "l600",
                "gamecube",
                "nuon",
                "ique player",
                "panasonic m2",
                "panasonic q",
                "playstation 2",
                "psx",
                "v.smile",
                "xavixport gaming console",
                "xbox",
                "atari flashback 2",
                "atari flashback 3",
                "atari flashback 4",
                "evo smart console",
                "retro duo",
                "game wave",
                "mattel hyperscan",
                "onlive",
                "phantom",
                "playstation 3",
                "wii",
                "xbox 360",
                "sega firecore",
                "zeebo",
                "sega zone",
                "eedoo ct510",
                "wii u",
                "ouya",
                "gamestick",
                "mojo",
                "gamepop",
                "playstation 4",
                "steam machine",
                "xbox one",
                "xi3 piston"
            };
        }

        public void makeelementslist()
        {
            elementslist = new[]
            {
                "hydrogen",
                "helium",
                "lithium",
                "beryllium",
                "boron",
                "carbon",
                "nitrogen",
                "oxygen",
                "fluorine",
                "neon",
                "sodium",
                "magnesium",
                "aluminium",
                "silicon",
                "phosphorus",
                "sulfur",
                "chlorine",
                "argon",
                "potassium",
                "calcium",
                "scandium",
                "titanium",
                "vanadium",
                "chromium",
                "manganese",
                "iron",
                "cobalt",
                "nickel",
                "copper",
                "zinc",
                "gallium",
                "germanium",
                "arsenic",
                "selenium",
                "bromine",
                "krypton",
                "rubidium",
                "strontium",
                "yttrium",
                "zirconium",
                "niobium",
                "molybdenum",
                "technetium",
                "ruthenium",
                "rhodium",
                "palladium",
                "silver",
                "cadmium",
                "indium",
                "tin",
                "antimony",
                "tellurium",
                "iodine",
                "xenon",
                "caesium",
                "barium",
                "lanthanum",
                "cerium",
                "praseodymium",
                "neodymium",
                "promethium",
                "samarium",
                "europium",
                "gadolinium",
                "terbium",
                "dysprosium",
                "holmium",
                "erbium",
                "thulium",
                "ytterbium",
                "lutetium",
                "hafnium",
                "tantalum",
                "tungsten",
                "rhenium",
                "osmium",
                "iridium",
                "platinum",
                "gold",
                "mercury",
                "thallium",
                "lead",
                "bismuth",
                "polonium",
                "astatine",
                "radon",
                "francium",
                "radium",
                "actinium",
                "thorium",
                "protactinium",
                "uranium",
                "neptunium",
                "plutonium",
                "americium",
                "curium",
                "berkelium",
                "californium",
                "einsteinium",
                "fermium",
                "mendelevium",
                "nobelium",
                "lawrencium",
                "rutherfordium",
                "dubnium",
                "seaborgium",
                "bohrium",
                "hassium",
                "meitnerium",
                "darmstadtium",
                "roentgenium",
                "copernicium",
                "ununtrium",
                "flerovium",
                "ununpentium",
                "livermorium",
                "ununseptium",
                "ununoctium"
            };
        }
    }
}
