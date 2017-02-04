Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports ShiftOS.Engine


Public Class Skins
#Region "Declarations"
    Dim firstrun As Boolean = True
    'WINDOW SETTINGS/IMAGES
    'images
    Public Shared titlebar As Image = Nothing
    <Image("titlebar")>
    Public Shared titlebarlayout As String = 3
    Public Shared borderleft As Image = Nothing
    <Image("leftborder")>
    Public Shared borderleftlayout As String = 3
    Public Shared borderright As Image = Nothing
    <Image("rightborder")>
    Public Shared borderrightlayout As String = 3
    Public Shared borderbottom As Image = Nothing
    <Image("bottomborder")>
    Public Shared borderbottomlayout As String = 3
    Public Shared closebtn As Image = Nothing
    <Image("closebutton")>
    Public Shared closebtnlayout As String = 3
    Public Shared closebtnhover As Image = Nothing
    Public Shared closebtnclick As Image = Nothing
    Public Shared rollbtn As Image = Nothing
    <Image("maximizebutton")>
    Public Shared rollbtnlayout As String = 3
    Public Shared rollbtnhover As Image = Nothing
    Public Shared rollbtnclick As Image = Nothing
    Public Shared minbtn As Image = Nothing
    <Image("minimizebutton")>
    Public Shared minbtnlayout As String = 3
    Public Shared minbtnhover As Image = Nothing
    Public Shared minbtnclick As Image = Nothing
    Public Shared rightcorner As Image = Nothing
    <Image("titleright")>
    Public Shared rightcornerlayout As String = 3
    Public Shared leftcorner As Image = Nothing
    <Image("titleleft")>
    Public Shared leftcornerlayout As String = 3
    ' Late entry: need to fix window code to include this
    Public Shared bottomleftcorner As Image = Nothing
    <Image("bottomlborder")>
    Public Shared bottomleftcornerlayout As String = 3
    Public Shared bottomrightcorner As Image = Nothing
    <Image("bottomrborder")>
    Public Shared bottomrightcornerlayout As String = 3
    Public Shared bottomleftcornercolour As Color = Color.Gray
    Public Shared bottomrightcornercolour As Color = Color.Gray

    Public Shared enablebordercorners As Boolean = False

    ' settings
    Public Shared closebtnsize As Size = New Size(22, 22)
    Public Shared rollbtnsize As Size = New Size(22, 22)
    Public Shared minbtnsize As Size = New Size(22, 22)
    Public Shared titlebarheight As Integer = 30
    Public Shared closebtnfromtop As Integer = 5
    Public Shared closebtnfromside As Integer = 2
    Public Shared rollbtnfromtop As Integer = 5
    Public Shared rollbtnfromside As Integer = 26
    Public Shared minbtnfromtop As Integer = 5
    Public Shared minbtnfromside As Integer = 52
    Public Shared borderwidth As Integer = 2
    Public Shared enablecorners As Boolean = False
    Public Shared titlebarcornerwidth As Integer = 5
    Public Shared titleiconfromside As Integer = 4
    Public Shared titleiconfromtop As Integer = 4
    'colours
    Public Shared titlebarcolour As Color = Color.Gray
    Public Shared borderleftcolour As Color = Color.Gray
    Public Shared borderrightcolour As Color = Color.Gray
    Public Shared borderbottomcolour As Color = Color.Gray
    Public Shared closebtncolour As Color = Color.Black
    Public Shared closebtnhovercolour As Color = Color.Black
    Public Shared closebtnclickcolour As Color = Color.Black
    Public Shared rollbtncolour As Color = Color.Black
    Public Shared rollbtnhovercolour As Color = Color.Black
    Public Shared rollbtnclickcolour As Color = Color.Black
    Public Shared minbtncolour As Color = Color.Black
    Public Shared minbtnhovercolour As Color = Color.Black
    Public Shared minbtnclickcolour As Color = Color.Black
    Public Shared rightcornercolour As Color = Color.Gray
    Public Shared leftcornercolour As Color = Color.Gray
    ' Text
    Public Shared titletextfontfamily As String = "Microsoft Sans Serif"
    Public Shared titletextfontsize As Integer = 10
    Public Shared titletextfontstyle As String = FontStyle.Bold
    Public Shared titletextpos As String = "Left"
    Public Shared titletextfromtop As Integer = 3
    Public Shared titletextfromside As Integer = 24
    Public Shared titletextcolour As Color = Color.White

    'DESKTOP
    Public Shared desktoppanelcolour As Color = Color.Gray
    Public Shared desktopbackgroundcolour As Color
    Public Shared desktoppanelheight As Integer = 24
    Public Shared desktoppanelposition As String = "Top"
    Public Shared clocktextcolour As Color = Color.Black
    Public Shared clockbackgroundcolor As Color = Color.Gray
    Public Shared panelclocktexttop As Integer = 3
    Public Shared panelclocktextsize As Integer = 10
    Public Shared panelclocktextfont As String = "Byington"
    Public Shared panelclocktextstyle As FontStyle = FontStyle.Bold
    Public Shared applauncherbuttoncolour As Color = Color.Gray
    Public Shared applauncherbuttonclickedcolour As Color = Color.Gray
    Public Shared applauncherbackgroundcolour As Color = Color.Gray
    Public Shared applaunchermouseovercolour As Color = Color.Gray
    Public Shared applicationsbuttontextcolour As Color = Color.Black
    Public Shared applicationbuttonheight As Integer = 24
    Public Shared applicationbuttontextsize As Integer = 10
    Public Shared applicationbuttontextfont As String = "Byington"
    Public Shared applicationbuttontextstyle As FontStyle = FontStyle.Bold
    Public Shared applicationlaunchername As String = "Applications"
    Public Shared titletextposition As String = "Left"
    Public Shared applaunchermenuholderwidth As Integer = 100
    Public Shared panelbuttonicontop As Integer = 3
    Public Shared panelbuttoniconside As Integer = 4
    Public Shared panelbuttoniconsize As Integer = 16
    Public Shared panelbuttonheight As Integer = 20
    Public Shared panelbuttonwidth As Integer = 185
    Public Shared panelbuttoncolour As Color = Color.Black
    Public Shared panelbuttontextcolour As Color = Color.White
    Public Shared panelbuttontextsize As Integer = 10
    Public Shared panelbuttontextfont As String = "Byington"
    Public Shared panelbuttontextstyle As FontStyle = FontStyle.Regular
    Public Shared panelbuttontextside As Integer = 16
    Public Shared panelbuttontexttop As Integer = 2
    Public Shared panelbuttongap As Integer = 4
    Public Shared panelbuttonfromtop As Integer = 2
    Public Shared panelbuttoninitialgap As Integer = 8

    Public Shared launcheritemsize As Integer = 10
    Public Shared launcheritemfont As String = "Byington"
    Public Shared launcheritemstyle As FontStyle = FontStyle.Regular
    Public Shared launcheritemcolour As Color = Color.Black

    ' Images
    Public Shared desktoppanel As Image = Nothing
    <Image("desktoppanel")>
    Public Shared desktoppanellayout As String = 3
    Public Shared desktopbackground As Image = Nothing
    <Image("desktopbackground")>
    Public Shared desktopbackgroundlayout As String = 3
    Public Shared panelclock As Image = Nothing
    <Image("panelclock")>
    Public Shared panelclocklayout As String = 3
    Public Shared applaunchermouseover As Image = Nothing
    Public Shared applauncher As Image = Nothing
    <Image("applauncher")>
    Public Shared applauncherlayout As String = 3
    Public Shared applauncherclick As Image = Nothing
    Public Shared panelbutton As Image = Nothing
    <Image("panelbutton")>
    Public Shared panelbuttonlayout As String = 3

    'Below is all for the Desktop Icons patch.

    Public Shared enabledraggableicons As Boolean = True
    Public Shared icontextcolor As Color = Color.White
    Public Shared showicons As Boolean = True
    Public Shared iconview1 As View = View.LargeIcon
    Public Shared iconview2 As View = View.Tile

    'DevX's Advanced App Launcher (coded by The Ultimate Hacker)

    Public Shared topBarHeight As Integer = 50
    Public Shared bottomBarHeight As Integer = 50
    Public Shared placesSide As String = "Left"
    Public Shared startHeight As Integer = 526
    Public Shared startWidth As Integer = 320
    Public Shared shutdownstring As String = "Shut Down ShiftOS"
    Public Shared userNamePosition = "Middle, Right"
    Public Shared recentIconsHorizontal As Boolean = False
    Public Shared usernametextcolor As Color = Color.White
    Public Shared usernamefont As String = "Trebuchet MS"
    Public Shared usernamefontsize As Integer = 12
    Public Shared usernamefontstyle As FontStyle = FontStyle.Bold
    Public Shared userNamePanelBackgroundColor As Color = Color.Gray
    Public Shared userNamePanelBackground As Image
    Public Shared powerPanelBackgroundColor As Color = Color.Gray
    Public Shared powerPanelBackgroundImage As Image
    Public Shared shutdownTextColor As Color = Color.White
    Public Shared shutdownTextFont As String = "Trebuchet MS"
    Public Shared shutdownTextSize As Integer = 12
    Public Shared shutdownTextStyle As FontStyle = FontStyle.Italic
    Public Shared usrPanelBackgroundLayout As ImageLayout = ImageLayout.Stretch
    Public Shared pwrPanelBackgroundLayout As ImageLayout = ImageLayout.Stretch
    Public Shared useClassicAppLauncher As Boolean = True


    '0.0.9 ALPHA 2

    'Login Screen

    Public Shared autologin As Boolean = True
    Public Shared fullScreen As Boolean = False
    Public Shared inputfont As String = "Trebuchet MS"
    Public Shared inputfontsize As Integer = 12
    Public Shared inputfontstyle As FontStyle = FontStyle.Regular
    Public Shared inputforecolor As Color = Color.Gray
    Public Shared inputbackcolor As Color = Color.Black
    Public Shared buttonfont As String = "Trebuchet MS"
    Public Shared buttonfontsize As Integer = 12
    Public Shared buttonfontstyle As FontStyle = FontStyle.Italic

    Public Shared userimagesize As Integer = 128
    Public Shared userimagelocation As Point = New Point(36, 202)
    Public Shared userimage As Image
    Public Shared userimagelayout As ImageLayout = ImageLayout.Stretch

    Public Shared loginbg As Image
    Public Shared loginbgcolor As Color = Color.Black
    Public Shared loginbglayout As ImageLayout = ImageLayout.Stretch

    'Locations...

    Public Shared userTextboxX As Integer = 171
    Public Shared userTextBoxY As Integer = 202
    Public Shared passTextBoxX As Integer = 171
    Public Shared passTextBoxY As Integer = 243
    Public Shared loginbtnX As Integer = 268
    Public Shared loginbtnY As Integer = 286
    Public Shared shutdownbtnX As Integer = 1755
    Public Shared shutdownbtnY As Integer = 979

    Public Shared Function GetLayout(id As String) As ImageLayout
        Dim type = GetType(Skins)
        For Each member As System.Reflection.FieldInfo In type.GetFields(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
            For Each attribute As Attribute In member.GetCustomAttributes(False)
                Try
                    Dim image As ImageAttribute = attribute
                    'WHY CAN'T WE HAVE A C#-LIKE Is OPERATOR, VB?
                    'I don't want to use a damn BODGE to check if my attribute is an ImageAttribute.
                    'Oh well.

                    If id = image.Name Then Return member.GetValue(Nothing)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next
        Next


        Console.WriteLine("Couldn't find layout of ID " + id + ". Assuming a tile layout.")
        Return ImageLayout.Tile
    End Function

    Private Shared Function GetImage(ByVal fileName As String) As Bitmap
        If Not File.Exists(fileName) Then Return Nothing

        Dim ret As Bitmap
        Using img As Image = Image.FromFile(fileName)
            ret = New Bitmap(img)
        End Using
        Console.WriteLine(fileName)
        Return ret
    End Function

#End Region

    Const loadedskin As String = "temp_skn/"

    ' LOAD SKIN FROM SAVE FOLDER
    Public Shared Sub loadimages()
        setupdefaults()

        If File.Exists(loadedskin + "startbutton.png") Then
            applauncher = GetImage(loadedskin + "taskbar.png")
            applauncherclick = GetImage(loadedskin + "taskbar.png")
            applaunchermouseover = GetImage(loadedskin + "taskbar.png")
        End If
        If File.Exists(loadedskin + "taskbar.png") Then
            desktoppanel = GetImage(loadedskin + "taskbar.png")
        End If
        If File.Exists(loadedskin + "background.png") Then
            desktopbackground = GetImage(loadedskin + "background.png")
        End If
        If File.Exists(loadedskin + "borders.png") Then
            borderleft = GetImage(loadedskin + "borders.png")
            borderright = GetImage(loadedskin + "borders.png")
            borderbottom = GetImage(loadedskin + "borders.png")
            bottomleftcorner = GetImage(loadedskin + "borders.png")
            bottomrightcorner = GetImage(loadedskin + "borders.png")
        End If
        If File.Exists(loadedskin + "closebutton.png") Then
            closebtn = GetImage(loadedskin + "closebutton.png")
        End If
        If File.Exists(loadedskin + "minimizebutton.png") Then
            minbtn = GetImage(loadedskin + "minimizebutton.png")
        End If
        If File.Exists(loadedskin + "panelbutton.png") Then
            panelbutton = GetImage(loadedskin + "panelbutton.png")
        End If
        If File.Exists(loadedskin + "rollup.png") Then
            rollbtn = GetImage(loadedskin + "rollup.png")
        End If
        If File.Exists(loadedskin + "titlebar.png") Then
            titlebar = GetImage(loadedskin + "titlebar.png")
        End If
        If File.Exists(loadedskin + "titlebarleft.png") Then
            leftcorner = GetImage(loadedskin + "titlebarleft.png")
        End If
        If File.Exists(loadedskin + "titlebarright.png") Then
            rightcorner = GetImage(loadedskin + "titlebarright.png")
        End If



        If File.Exists(loadedskin & "userpic") Then
            userimage = GetImage(loadedskin & "userpic")
        End If
        If File.Exists(loadedskin & "loginbg") Then
            loginbg = GetImage(loadedskin & "loginbg")
        End If
        If File.Exists(loadedskin & "userbar") Then
            userNamePanelBackground = GetImage(loadedskin & "userbar")
        End If
        If File.Exists(loadedskin & "powerbar") Then
            powerPanelBackgroundImage = GetImage(loadedskin & "powerbar")
        End If
        If File.Exists(loadedskin & "titlebar") Then
            titlebar = GetImage(loadedskin & "titlebar")
        End If
        If File.Exists(loadedskin & "borderleft") Then
            borderleft = GetImage(loadedskin & "borderleft")
        End If
        If File.Exists(loadedskin & "borderright") Then
            borderright = GetImage(loadedskin & "borderright".Clone)
        End If
        If File.Exists(loadedskin & "borderbottom") Then
            borderbottom = GetImage(loadedskin & "borderbottom".Clone)
        End If
        If File.Exists(loadedskin & "closebtn") Then
            closebtn = GetImage(loadedskin & "closebtn".Clone)
        End If
        If File.Exists(loadedskin & "closebtnhover") Then
            closebtnhover = GetImage(loadedskin & "closebtnhover".Clone)
        End If
        If File.Exists(loadedskin & "closebtnclick") Then
            closebtnclick = GetImage(loadedskin & "closebtnclick".Clone)
        End If
        If File.Exists(loadedskin & "rollbtn") Then
            rollbtn = GetImage(loadedskin & "rollbtn".Clone)
        End If
        If File.Exists(loadedskin & "rollbtnhover") Then
            rollbtnhover = GetImage(loadedskin & "rollbtnhover".Clone)
        End If
        If File.Exists(loadedskin & "rollbtnclick") Then
            rollbtnclick = GetImage(loadedskin & "rollbtnclick".Clone)
        End If
        If File.Exists(loadedskin & "minbtn") Then
            minbtn = GetImage(loadedskin & "minbtn".Clone)
        End If
        If File.Exists(loadedskin & "minbtnhover") Then
            minbtnhover = GetImage(loadedskin & "minbtnhover".Clone)
        End If
        If File.Exists(loadedskin & "minbtnclick") Then
            minbtnclick = GetImage(loadedskin & "minbtnclick".Clone)
        End If
        If File.Exists(loadedskin & "rightcorner") Then
            rightcorner = GetImage(loadedskin & "rightcorner".Clone)
        End If
        If File.Exists(loadedskin & "leftcorner") Then
            leftcorner = GetImage(loadedskin & "leftcorner".Clone)
        End If
        If File.Exists(loadedskin & "desktoppanel") Then
            desktoppanel = GetImage(loadedskin & "desktoppanel".Clone)
        End If
        If File.Exists(loadedskin & "desktopbackground") Then
            desktopbackground = GetImage(loadedskin & "desktopbackground".Clone)
        End If
        If File.Exists(loadedskin & "panelbutton") Then
            panelbutton = GetImage(loadedskin & "panelbutton".Clone)
        End If
        If File.Exists(loadedskin & "applaunchermouseover") Then
            applaunchermouseover = GetImage(loadedskin & "applaunchermouseover".Clone)
        End If
        If File.Exists(loadedskin & "applauncher") Then
            applauncher = GetImage(loadedskin & "applauncher".Clone)
        End If
        If File.Exists(loadedskin & "applauncherclick") Then
            applauncherclick = GetImage(loadedskin & "applauncherclick".Clone)
        End If
        If File.Exists(loadedskin & "panelclock") Then
            panelclock = GetImage(loadedskin & "panelclock".Clone)
        End If
        If File.Exists(loadedskin & "bottomleftcorner") Then
            bottomleftcorner = GetImage(loadedskin & "bottomleftcorner".Clone)
        End If
        If File.Exists(loadedskin & "bottomrightcorner") Then
            bottomrightcorner = GetImage(loadedskin & "bottomrightcorner".Clone)
        End If
        'load settings
        Dim loaddata(200) As String
        If File.Exists(loadedskin & "data.dat") Then
            Dim sr As StreamReader = New StreamReader(loadedskin & "data.dat")

            For i As Integer = 0 To 200 Step 1
                loaddata(i) = sr.ReadLine
                If i = 200 Then
                    sr.Close()
                    Exit For
                End If
            Next
        End If

        If File.Exists(loadedskin + "skindata.dat") Then
            LoadSkin007()
            Return
        End If
        ' settings

        If File.Exists(loadedskin + "data.json") Then
            'UH OH
            Infobox.Show("Modern skin detected!", "You are attempting to convert a modern skin to a modern skin! Perhaps you mean to load it in Skin Loader?")
            Return
        End If

        closebtnsize = New Size(loaddata(1), loaddata(2))
        rollbtnsize = New Size(loaddata(3), loaddata(4))
        minbtnsize = New Size(loaddata(5), loaddata(6))
        titlebarheight = loaddata(7)
        closebtnfromtop = loaddata(8)
        closebtnfromside = loaddata(9)
        rollbtnfromtop = loaddata(10)
        rollbtnfromside = loaddata(11)
        minbtnfromtop = loaddata(12)
        minbtnfromside = loaddata(13)
        borderwidth = loaddata(14)
        enablecorners = loaddata(15)
        titlebarcornerwidth = loaddata(16)
        titleiconfromside = loaddata(17)
        titleiconfromtop = loaddata(18)
        titlebarcolour = Color.FromArgb(loaddata(19))
        borderleftcolour = Color.FromArgb(loaddata(20))
        borderrightcolour = Color.FromArgb(loaddata(21))
        borderbottomcolour = Color.FromArgb(loaddata(22))
        closebtncolour = Color.FromArgb(loaddata(23))
        closebtnhovercolour = Color.FromArgb(loaddata(24))
        closebtnclickcolour = Color.FromArgb(loaddata(25))
        rollbtncolour = Color.FromArgb(loaddata(26))
        rollbtnhovercolour = Color.FromArgb(loaddata(27))
        rollbtnclickcolour = Color.FromArgb(loaddata(28))
        minbtncolour = Color.FromArgb(loaddata(29))
        minbtnhovercolour = Color.FromArgb(loaddata(30))
        minbtnclickcolour = Color.FromArgb(loaddata(31))
        rightcornercolour = Color.FromArgb(loaddata(32))
        leftcornercolour = Color.FromArgb(loaddata(33))
        bottomrightcornercolour = Color.FromArgb(loaddata(34))
        bottomleftcornercolour = Color.FromArgb(loaddata(35))
        titletextfontfamily = loaddata(36)
        titletextfontsize = loaddata(37)
        titletextfontstyle = loaddata(38)
        titletextpos = loaddata(39)
        titletextfromtop = loaddata(40)
        titletextfromside = loaddata(41)
        titletextcolour = Color.FromArgb(loaddata(42))
        desktoppanelcolour = Color.FromArgb(loaddata(43))
        desktopbackgroundcolour = Color.FromArgb(loaddata(44))
        desktoppanelheight = loaddata(45)
        desktoppanelposition = loaddata(46)
        clocktextcolour = Color.FromArgb(loaddata(47))
        clockbackgroundcolor = Color.FromArgb(loaddata(48))
        panelclocktexttop = loaddata(49)
        panelclocktextsize = loaddata(50)
        panelclocktextfont = loaddata(51)
        panelclocktextstyle = loaddata(52)
        applauncherbuttoncolour = Color.FromArgb(loaddata(53))
        applauncherbuttonclickedcolour = Color.FromArgb(loaddata(54))
        applauncherbackgroundcolour = Color.FromArgb(loaddata(55))
        applaunchermouseovercolour = Color.FromArgb(loaddata(56))
        applicationsbuttontextcolour = Color.FromArgb(loaddata(57))
        applicationbuttonheight = loaddata(58)
        applicationbuttontextsize = loaddata(59)
        applicationbuttontextfont = loaddata(60)
        applicationbuttontextstyle = loaddata(61)
        applicationlaunchername = loaddata(62)
        titletextposition = loaddata(63)
        applaunchermenuholderwidth = loaddata(64)
        panelbuttonicontop = loaddata(65)
        panelbuttoniconside = loaddata(66)
        panelbuttoniconsize = loaddata(67)
        panelbuttonheight = loaddata(68)
        panelbuttonwidth = loaddata(69)
        panelbuttoncolour = Color.FromArgb(loaddata(70))
        panelbuttontextcolour = Color.FromArgb(loaddata(71))
        panelbuttontextsize = loaddata(72)
        panelbuttontextfont = loaddata(73)
        panelbuttontextstyle = loaddata(74)
        panelbuttontextside = loaddata(75)
        panelbuttontexttop = loaddata(76)
        panelbuttongap = loaddata(77)
        panelbuttonfromtop = loaddata(78)
        panelbuttoninitialgap = loaddata(79)

        'layout stuff
        titlebarlayout = loaddata(89)
        borderleftlayout = loaddata(90)
        borderrightlayout = loaddata(91)
        borderbottomlayout = loaddata(92)
        closebtnlayout = loaddata(93)
        rollbtnlayout = loaddata(94)
        minbtnlayout = loaddata(95)
        rightcornerlayout = loaddata(96)
        leftcornerlayout = loaddata(97)
        desktoppanellayout = loaddata(98)
        desktopbackgroundlayout = loaddata(99)
        panelclocklayout = loaddata(100)
        applauncherlayout = loaddata(101)
        panelbuttonlayout = loaddata(102)
        bottomleftcornerlayout = loaddata(103)
        bottomrightcornerlayout = loaddata(104)
        ' End of 0.0.8 beta 6 save file, check if exists for future features 
        If Not loaddata(105) = "" Then launcheritemcolour = Color.FromArgb(loaddata(105))
        If Not loaddata(106) = "" Then launcheritemfont = loaddata(106)
        If Not loaddata(107) = "" Then launcheritemsize = loaddata(107)
        If Not loaddata(108) = "" Then launcheritemstyle = loaddata(108)
        If Not loaddata(109) = "" Then enablebordercorners = loaddata(109)

        'for adding extra features, check:

        If loaddata(110) = "" Or loaddata(110) = "End of skin data" Then loaddata(110) = enabledraggableicons Else enabledraggableicons = loaddata(110)
        If loaddata(111) = "" Or loaddata(111) = "End of skin data" Then loaddata(111) = icontextcolor.ToArgb Else icontextcolor = Color.FromArgb(loaddata(111))
        If loaddata(112) = "" Or loaddata(112) = "End of skin data" Then loaddata(112) = showicons Else showicons = loaddata(112)
        If loaddata(113) = "" Or loaddata(113) = "End of skin data" Then loaddata(113) = iconview1 Else iconview1 = loaddata(113)
        Try
            If loaddata(114) = "" Then topBarHeight = 50 Else topBarHeight = loaddata(114)
        Catch ex As Exception
            topBarHeight = 50

        End Try
        If loaddata(115) = "" Then bottomBarHeight = 50 Else bottomBarHeight = loaddata(115)
        If loaddata(116) = "" Then placesSide = "Left" Else placesSide = loaddata(116)
        If loaddata(117) = "" Then startHeight = 526 Else startHeight = loaddata(117)
        If loaddata(118) = "" Then startWidth = 320 Else startWidth = loaddata(118)
        If loaddata(119) = "" Then shutdownstring = "Shut Down ShiftOS" Else shutdownstring = loaddata(119)
        If loaddata(120) = "" Then userNamePosition = "Middle, Right" Else userNamePosition = loaddata(120)
        If loaddata(121) = "" Then recentIconsHorizontal = False Else recentIconsHorizontal = loaddata(121)
        If loaddata(122) = "" Then usernametextcolor = Color.White Else usernametextcolor = Color.FromArgb(loaddata(122))
        If loaddata(123) = "" Then usernamefont = "Trebuchet MS" Else usernamefont = loaddata(123)
        If loaddata(124) = "" Then usernamefontsize = 12 Else usernamefontsize = loaddata(124)
        If loaddata(125) = "" Then usernamefontstyle = FontStyle.Bold Else usernamefontstyle = loaddata(125)
        If loaddata(126) = "" Then userNamePanelBackgroundColor = Color.Gray Else userNamePanelBackgroundColor = Color.FromArgb(loaddata(126))
        If loaddata(127) = "" Then powerPanelBackgroundColor = Color.Gray Else powerPanelBackgroundColor = Color.FromArgb(loaddata(127))
        If loaddata(128) = "" Then shutdownTextColor = Color.White Else shutdownTextColor = Color.FromArgb(loaddata(128))
        If loaddata(129) = "" Then shutdownTextFont = "Trebuchet MS" Else shutdownTextFont = loaddata(129)
        If loaddata(130) = "" Then shutdownTextSize = 12 Else shutdownTextSize = loaddata(130)
        If loaddata(131) = "" Then shutdownTextStyle = FontStyle.Italic Else shutdownTextStyle = loaddata(132)
        If loaddata(132) = "" Then usrPanelBackgroundLayout = ImageLayout.Stretch Else usrPanelBackgroundLayout = loaddata(132)
        If loaddata(133) = "" Then pwrPanelBackgroundLayout = ImageLayout.Stretch Else pwrPanelBackgroundLayout = loaddata(133)
        If loaddata(134) = "" Then useClassicAppLauncher = False Else useClassicAppLauncher = loaddata(134)
        If loaddata(135) = "" Then autologin = True Else autologin = loaddata(135)
        If loaddata(136) = "" Then fullScreen = False Else fullScreen = loaddata(136)
        If loaddata(137) = "" Then inputfont = "Trebuchet MS" Else inputfont = loaddata(137)
        If loaddata(138) = "" Then inputfontsize = 12 Else inputfontsize = loaddata(138)
        If loaddata(139) = "" Then inputfontstyle = FontStyle.Regular Else inputfontstyle = loaddata(139)
        If loaddata(140) = "" Then inputforecolor = Color.Gray Else inputforecolor = Color.FromArgb(loaddata(140))
        If loaddata(141) = "" Then inputbackcolor = Color.Black Else inputbackcolor = Color.FromArgb(loaddata(141))
        If loaddata(142) = "" Then buttonfont = "Trebuchet MS" Else buttonfont = loaddata(142)
        If loaddata(143) = "" Then buttonfontsize = 12 Else buttonfontsize = loaddata(143)
        If loaddata(144) = "" Then buttonfontstyle = FontStyle.Italic Else buttonfontstyle = loaddata(144)
        If loaddata(145) = "" Then userimagesize = 128 Else userimagesize = loaddata(145)
        If loaddata(146) = "" And loaddata(147) = "" Then userimagelocation = New Point(36, 202) Else userimagelocation = New Point(loaddata(146), loaddata(147))
        If loaddata(148) = "" Then userimagelayout = ImageLayout.Stretch Else userimagelayout = loaddata(148)
        If loaddata(149) = "" Then loginbgcolor = Color.Black Else loginbgcolor = Color.FromArgb(loaddata(149))
        If loaddata(150) = "" Then loginbglayout = ImageLayout.Stretch Else loginbglayout = loaddata(150)
    End Sub

    Public Shared Sub LoadSkin007()
        Dim loadlines = File.ReadAllLines(loadedskin + "skindata.dat")
        titlebarcolour = Color.FromArgb(loadlines(0))
        borderleftcolour = Color.FromArgb((loadlines(1)))
        borderrightcolour = Color.FromArgb((loadlines(1)))
        borderbottomcolour = Color.FromArgb((loadlines(1)))
        bottomleftcornercolour = Color.FromArgb((loadlines(1)))
        bottomrightcornercolour = Color.FromArgb((loadlines(1)))
        borderwidth = (loadlines(2))
        titlebarheight = (loadlines(3))
        closebtncolour = Color.FromArgb((loadlines(4)))
        closebtnsize = New Size(loadlines(5), loadlines(6))
        closebtnfromside = (loadlines(7))
        closebtnfromtop = (loadlines(8))
        titletextcolour = Color.FromArgb((loadlines(9)))
        titletextfromtop = (loadlines(10))
        titletextfromside = (loadlines(11))
        titletextfontsize = (loadlines(12))
        titletextfontfamily = loadlines(13)
        titletextfontstyle = CType((loadlines(14)), FontStyle)
        desktoppanelcolour = Color.FromArgb((loadlines(15)))
        desktopbackgroundcolour = Color.FromArgb((loadlines(16)))
        desktoppanelheight = (loadlines(17))
        desktoppanelposition = loadlines(18)
        clocktextcolour = Color.FromArgb((loadlines(19)))
        clockbackgroundcolor = Color.FromArgb((loadlines(20)))
        panelclocktexttop = (loadlines(21))
        panelclocktextsize = (loadlines(22))
        panelclocktextfont = loadlines(23)
        panelclocktextstyle = CType((loadlines(24)), FontStyle)
        applauncherbuttoncolour = Color.FromArgb((loadlines(25)))
        applauncherbuttonclickedcolour = Color.FromArgb((loadlines(26)))
        applauncherbackgroundcolour = Color.FromArgb((loadlines(27)))
        applaunchermouseovercolour = Color.FromArgb((loadlines(28)))
        applicationsbuttontextcolour = Color.FromArgb((loadlines(29)))
        applicationbuttonheight = (loadlines(30))
        applicationbuttontextsize = (loadlines(31))
        applicationbuttontextfont = loadlines(32)
        applicationbuttontextstyle = CType((loadlines(33)), FontStyle)
        applicationlaunchername = loadlines(34)
        titletextposition = loadlines(35)
        rollbtncolour = Color.FromArgb((loadlines(36)))
        If Not loadlines(37) = "" AndAlso Not loadlines(38) = "" Then
            rollbtnsize = New Size(loadlines(37), loadlines(38))
        End If
        If Not loadlines(39) = "" Then
            rollbtnfromside = (loadlines(39))
        End If
        If Not loadlines(40) = "" Then
            rollbtnfromtop = (loadlines(40))
        End If
        If Not loadlines(41) = "" Then
            titleiconfromside = (loadlines(41))
        End If
        If Not loadlines(42) = "" Then
            titleiconfromtop = (loadlines(42))
        End If
        If Not loadlines(44) = "" Then
            titlebarcornerwidth = (loadlines(44))
        End If
        If Not loadlines(45) = "" Then
            rightcornercolour = Color.FromArgb((loadlines(45)))
        End If
        If Not loadlines(46) = "" Then
            leftcornercolour = Color.FromArgb((loadlines(46)))
        End If
        If Not loadlines(47) = "" Then
            applaunchermenuholderwidth = (loadlines(47))
        End If
        If Not loadlines(48) = "" Then
            borderleftcolour = Color.FromArgb((loadlines(48)))
        End If
        If Not loadlines(49) = "" Then
            borderrightcolour = Color.FromArgb((loadlines(49)))
        End If
        If Not loadlines(50) = "" Then
            borderbottomcolour = Color.FromArgb((loadlines(50)))
        End If
        If Not loadlines(51) = "" Then
            bottomrightcornercolour = Color.FromArgb((loadlines(51)))
        End If
        If Not loadlines(52) = "" Then
            bottomleftcornercolour = Color.FromArgb((loadlines(52)))
        End If
        If Not loadlines(53) = "" Then
            panelbuttonicontop = (loadlines(53))
        End If
        If Not loadlines(54) = "" Then
            panelbuttoniconside = (loadlines(54))
        End If
        If Not loadlines(55) = "" Then
            panelbuttoniconsize = loadlines(55)

        End If
        If Not loadlines(57) = "" Then
            panelbuttonheight = (loadlines(57))
        End If
        If Not loadlines(58) = "" Then
            panelbuttonwidth = (loadlines(58))
        End If
        If Not loadlines(59) = "" Then
            panelbuttoncolour = Color.FromArgb((loadlines(59)))
        End If
        If Not loadlines(60) = "" Then
            panelbuttontextcolour = Color.FromArgb((loadlines(60)))
        End If
        If Not loadlines(61) = "" Then
            panelbuttontextsize = (loadlines(61))
        End If
        If Not loadlines(62) = "" Then
            panelbuttontextfont = loadlines(62)
        End If
        If Not loadlines(63) = "" Then
            panelbuttontextstyle = CType((loadlines(63)), FontStyle)
        End If
        If Not loadlines(64) = "" Then
            panelbuttontextside = (loadlines(64))
        End If
        If Not loadlines(65) = "" Then
            panelbuttontexttop = (loadlines(65))
        End If
        If Not loadlines(66) = "" Then
            panelbuttongap = (loadlines(66))
        End If
        If Not loadlines(67) = "" Then
            panelbuttonfromtop = (loadlines(67))
        End If
        If Not loadlines(68) = "" Then
            panelbuttoninitialgap = (loadlines(68))
        End If
        If Not loadlines(69) = "" Then
            minbtncolour = Color.FromArgb((loadlines(69)))
        End If
        If Not loadlines(70) = "" AndAlso Not loadlines(71) = "" Then
            minbtnsize = New Size(loadlines(70), loadlines(71))
        End If
        If Not loadlines(72) = "" Then
            minbtnfromside = (loadlines(72))
        End If
        If Not loadlines(73) = "" Then
            minbtnfromtop = (loadlines(73))
        End If
        Dim skinimages(50) As String

        skinimages(0) = loadlines(100)
        skinimages(1) = loadlines(101)
        skinimages(2) = loadlines(102)
        skinimages(3) = loadlines(103)
        skinimages(4) = loadlines(104)
        skinimages(5) = loadlines(105)
        skinimages(6) = loadlines(106)
        skinimages(7) = loadlines(107)
        skinimages(8) = loadlines(108)
        skinimages(9) = loadlines(109)
        skinimages(10) = loadlines(110)
        skinimages(11) = loadlines(111)
        skinimages(12) = loadlines(112)
        skinimages(13) = loadlines(113)
        skinimages(14) = loadlines(114)
        skinimages(15) = loadlines(115)
        skinimages(16) = loadlines(116)
        skinimages(17) = loadlines(117)
        skinimages(18) = loadlines(118)
        skinimages(19) = loadlines(119)
        skinimages(20) = loadlines(120)
        skinimages(21) = loadlines(121)
        skinimages(22) = loadlines(122)
        skinimages(23) = loadlines(123)
        skinimages(24) = loadlines(124)
        skinimages(25) = loadlines(125)
        skinimages(26) = loadlines(126)
        skinimages(27) = loadlines(127)
        skinimages(28) = loadlines(128)
        skinimages(29) = loadlines(129)
        skinimages(30) = loadlines(130)
        skinimages(31) = loadlines(131)
        skinimages(32) = loadlines(132)
        skinimages(33) = loadlines(133)
        skinimages(34) = loadlines(134)
        skinimages(35) = loadlines(135)
        skinimages(36) = loadlines(136)
        skinimages(37) = loadlines(137)
        skinimages(38) = loadlines(138)
        skinimages(39) = loadlines(139)
        skinimages(40) = loadlines(140)
        skinimages(41) = loadlines(141)
        skinimages(42) = loadlines(142)
        skinimages(43) = loadlines(143)
        skinimages(44) = loadlines(144)
        skinimages(45) = loadlines(145)
        skinimages(46) = loadlines(146)
        skinimages(47) = loadlines(147)
        skinimages(48) = loadlines(148)
        skinimages(49) = loadlines(149)
        skinimages(50) = loadlines(150)

        closebtn = GetImage(skinimages(0))
        titlebar = GetImage(skinimages(3))
        desktopbackground = GetImage(skinimages(6))
        rollbtn = GetImage(skinimages(9))
        rightcorner = GetImage(skinimages(12))
        leftcorner = GetImage(skinimages(15))
        desktoppanel = GetImage(skinimages(18))
        panelclock = GetImage(skinimages(21))
        applauncher = GetImage(skinimages(24))
        borderleft = GetImage(skinimages(27))
        borderright = GetImage(skinimages(30))
        borderbottom = GetImage(skinimages(33))
        bottomrightcorner = GetImage(skinimages(36))
        bottomleftcorner = GetImage(skinimages(39))
        minbtn = GetImage(skinimages(42))

    End Sub

#If False Then
    Public Sub setpreviewtocurrentskin()
			Dim num2 As Integer
			Dim num3 As Integer
			Try
				Me.prepnlpanelbutton.BackgroundImage = Me.skinpanelbutton(0)
				Me.ApplicationsToolStripMenuItem.BackgroundImage = Me.skinapplauncherbutton(0)
				Me.preclosebutton.BackgroundImage = Image.FromFile(Me.skinimages(0))
				Me.pretitlebar.BackgroundImage = Image.FromFile(Me.skinimages(3))
				Me.pnldesktoppreview.BackgroundImage = Image.FromFile(Me.skinimages(6))
				Me.prerollupbutton.BackgroundImage = Image.FromFile(Me.skinimages(9))
				Me.prepgtoprcorner.BackgroundImage = Image.FromFile(Me.skinimages(12))
				Me.prepgtoplcorner.BackgroundImage = Image.FromFile(Me.skinimages(15))
				Me.predesktoppanel.BackgroundImage = Image.FromFile(Me.skinimages(18))
				Me.pretimepanel.BackgroundImage = Image.FromFile(Me.skinimages(21))
				Me.ApplicationsToolStripMenuItem.BackgroundImage = Image.FromFile(Me.skinimages(24))
				Me.prepgleft.BackgroundImage = Image.FromFile(Me.skinimages(27))
				Me.prepgright.BackgroundImage = Image.FromFile(Me.skinimages(30))
				Me.prepgbottom.BackgroundImage = Image.FromFile(Me.skinimages(33))
				Me.prepgbottomrcorner.BackgroundImage = Image.FromFile(Me.skinimages(36))
				Me.prepgbottomlcorner.BackgroundImage = Image.FromFile(Me.skinimages(39))
				Me.preminimizebutton.BackgroundImage = Image.FromFile(Me.skinimages(42))
				Me.prepnlpanelbutton.BackgroundImage = Image.FromFile(Me.skinimages(45))
				Me.Invalidate()
				
			End Try
		End Sub
#End If

    Public Shared Sub setupdefaults()
        titlebar = Nothing
        titlebarlayout = 3
        borderleft = Nothing
        borderleftlayout = 3
        borderright = Nothing
        borderrightlayout = 3
        borderbottom = Nothing
        borderbottomlayout = 3
        closebtn = Nothing
        closebtnlayout = 3
        closebtnhover = Nothing
        closebtnclick = Nothing
        rollbtn = Nothing
        rollbtnlayout = 3
        rollbtnhover = Nothing
        rollbtnclick = Nothing
        minbtn = Nothing
        minbtnlayout = 3
        minbtnhover = Nothing
        minbtnclick = Nothing
        rightcorner = Nothing
        rightcornerlayout = 3
        leftcorner = Nothing
        leftcornerlayout = 3
        bottomleftcorner = Nothing
        bottomleftcornerlayout = 3
        bottomrightcorner = Nothing
        bottomrightcornerlayout = 3
        bottomleftcornercolour = Color.Gray
        bottomrightcornercolour = Color.Gray
        enablebordercorners = False
        closebtnsize = New Size(22, 22)
        rollbtnsize = New Size(22, 22)
        minbtnsize = New Size(22, 22)
        titlebarheight = 30
        closebtnfromtop = 5
        closebtnfromside = 2
        rollbtnfromtop = 5
        rollbtnfromside = 26
        minbtnfromtop = 5
        minbtnfromside = 52
        borderwidth = 2
        enablecorners = False
        titlebarcornerwidth = 5
        titleiconfromside = 4
        titleiconfromtop = 4
        titlebarcolour = Color.Gray
        borderleftcolour = Color.Gray
        borderrightcolour = Color.Gray
        borderbottomcolour = Color.Gray
        closebtncolour = Color.Black
        closebtnhovercolour = Color.Black
        closebtnclickcolour = Color.Black
        rollbtncolour = Color.Black
        rollbtnhovercolour = Color.Black
        rollbtnclickcolour = Color.Black
        minbtncolour = Color.Black
        minbtnhovercolour = Color.Black
        minbtnclickcolour = Color.Black
        rightcornercolour = Color.Gray
        leftcornercolour = Color.Gray
        titletextfontfamily = "Microsoft Sans Serif"
        titletextfontsize = 10
        titletextfontstyle = FontStyle.Bold
        titletextpos = "Left"
        titletextfromtop = 3
        titletextfromside = 24
        titletextcolour = Color.White
        desktoppanelcolour = Color.Gray
        desktopbackgroundcolour = Color.Black
        desktoppanelheight = 24
        desktoppanelposition = "Top"
        clocktextcolour = Color.Black
        clockbackgroundcolor = Color.Gray
        panelclocktexttop = 3
        panelclocktextsize = 10
        panelclocktextfont = "Byington"
        panelclocktextstyle = FontStyle.Bold
        applauncherbuttoncolour = Color.Gray
        applauncherbuttonclickedcolour = Color.Gray
        applauncherbackgroundcolour = Color.Gray
        applaunchermouseovercolour = Color.Gray
        applicationsbuttontextcolour = Color.Black
        applicationbuttonheight = 24
        applicationbuttontextsize = 10
        applicationbuttontextfont = "Byington"
        applicationbuttontextstyle = FontStyle.Bold
        applicationlaunchername = "Applications"
        titletextposition = "Left"
        applaunchermenuholderwidth = 100
        panelbuttonicontop = 3
        panelbuttoniconside = 4
        panelbuttoniconsize = 16
        panelbuttonheight = 20
        panelbuttonwidth = 185
        panelbuttoncolour = Color.Black
        panelbuttontextcolour = Color.White
        panelbuttontextsize = 10
        panelbuttontextfont = "Byington"
        panelbuttontextstyle = FontStyle.Regular
        panelbuttontextside = 16
        panelbuttontexttop = 2
        panelbuttongap = 4
        panelbuttonfromtop = 2
        panelbuttoninitialgap = 8
        launcheritemsize = 10
        launcheritemfont = "Byington"
        launcheritemstyle = FontStyle.Regular
        launcheritemcolour = Color.Black
        desktoppanel = Nothing
        desktoppanellayout = 3
        desktopbackground = Nothing
        desktopbackgroundlayout = 3
        panelclock = Nothing
        panelclocklayout = 3
        applaunchermouseover = Nothing
        applauncher = Nothing
        applauncherlayout = 3
        applauncherclick = Nothing
        panelbutton = Nothing
        panelbuttonlayout = 3
        enabledraggableicons = True
    End Sub
End Class