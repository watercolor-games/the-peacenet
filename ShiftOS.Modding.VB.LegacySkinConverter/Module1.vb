Imports ShiftOS.Engine
Imports ShiftOS.Objects
Imports ShiftOS.Objects.ShiftFS
Imports System.IO
Imports System.IO.Compression
Imports System.Collections.Generic
Imports System.Drawing

Module Module1

    Sub Main()
        ShiftOS.WinForms.Program.Main()
    End Sub

End Module

<ShiftOS.Engine.Namespace("skinning")>
Public Class SkinConverterCommands


    <Command("convert", description:="Converts a specified legacy Skin File to a modern one usable in Skin Loader.")>
    <RequiresArgument("in")>
    <RequiresArgument("out")>
    <RequiresUpgrade("skinning")>
    Public Shared Function Convert(args As Dictionary(Of String, Object))
        Dim input = args("in").ToString()
        Dim output = args("out").ToString()

        Dim bytes As Byte() = Utils.ReadAllBytes(input)
        System.IO.File.WriteAllBytes("temp.skn", bytes)

        If System.IO.Directory.Exists("temp_skn") Then
            System.IO.Directory.Delete("temp_skn", True)
        End If
        Console.WriteLine("Cleaning environment...")
        System.IO.Directory.CreateDirectory("temp_skn")

        ZipFile.ExtractToDirectory("temp.skn", "temp_skn")
        Console.WriteLine("Extracted skin... loading it now.")
        loadimages()
        Dim skn = New Skin()

        'Get images.

        Console.Write("Processing images...")
        skn.DesktopBackgroundImage = SaveImageToBinary(desktopbackground)
        skn.DesktopPanelBackground = SaveImageToBinary(desktoppanel)
        skn.CloseButtonImage = SaveImageToBinary(closebtn)
        skn.MaximizeButtonImage = SaveImageToBinary(rollbtn)
        skn.MinimizeButtonImage = SaveImageToBinary(minbtn)
        skn.PanelButtonBG = SaveImageToBinary(panelbutton)
        skn.AppLauncherImage = SaveImageToBinary(applauncher)
        skn.TitleBarBackground = SaveImageToBinary(titlebar)
        skn.TitleLeftCornerWidth = titlebarcornerwidth
        skn.TitleRightCornerWidth = titlebarcornerwidth
        skn.ShowTitleCorners = True 'I don't know what the legacy version of this is.
        skn.TitleLeftBG = SaveImageToBinary(leftcorner)
        skn.TitleRightBG = SaveImageToBinary(rightcorner)
        skn.LeftBorderBG = SaveImageToBinary(borderleft)
        skn.RightBorderBG = SaveImageToBinary(borderright)
        skn.BottomBorderBG = SaveImageToBinary(borderbottom)
        skn.BottomLBorderBG = SaveImageToBinary(bottomleftcorner)
        skn.BottomRBorderBG = SaveImageToBinary(bottomrightcorner)

        Console.WriteLine(" ...done.")

        Console.Write("Converting desktop settings...")

        skn.AppLauncherText = Skins.applicationlaunchername
        skn.AppLauncherHolderSize = New Size(applaunchermenuholderwidth, desktoppanelheight)
        skn.AppLauncherFromLeft = New Point(0, 0)
        skn.DesktopPanelHeight = desktoppanelheight
        Select Case desktoppanelposition
            Case "Top"
                skn.DesktopPanelPosition = 0
            Case "Bottom"
                skn.DesktopPanelPosition = 1
        End Select
        skn.DesktopPanelColor = desktoppanelcolour
        skn.DesktopPanelClockBackgroundColor = clockbackgroundcolor
        skn.DesktopPanelClockColor = clocktextcolour
        skn.DesktopPanelClockFont = New Font(panelclocktextfont, panelclocktextsize, panelclocktextstyle)
        skn.PanelButtonColor = panelbuttoncolour
        skn.PanelButtonTextColor = panelbuttontextcolour
        skn.PanelButtonFont = New Font(panelbuttontextfont, panelbuttontextsize, panelbuttontextstyle)
        skn.PanelButtonHolderFromLeft = panelbuttoninitialgap
        skn.PanelButtonSize = New Size(panelbuttonwidth, panelbuttonheight)
        skn.PanelButtonFromLeft = New Point(panelbuttongap, panelbuttonfromtop)
        skn.DesktopPanelClockFromRight = New Point(0, 0)

        Console.WriteLine(" ...done")

        Utils.WriteAllText(output, skn.ToString())
        Infobox.Show("Skin converted!", "We have successfully converted your skin and saved it to " + output + ". Go ahead and load it!")
        Return True
    End Function


    Private Shared Function SaveImageToBinary(img As Image) As Byte()
        If img Is Nothing Then Return Nothing

        Using mStream As New MemoryStream
            img.Save(mStream, Imaging.ImageFormat.Png)
            Return mStream.ToArray()
        End Using
    End Function
End Class
