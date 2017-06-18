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
        Skins.loadimages()
        Dim skn = New Skin()

        'Get images.

        Console.Write("Processing images...")
        skn.DesktopBackgroundImage = SaveImageToBinary(Skins.desktopbackground)
        skn.DesktopPanelBackground = SaveImageToBinary(Skins.desktoppanel)
        skn.CloseButtonImage = SaveImageToBinary(Skins.closebtn)
        skn.MaximizeButtonImage = SaveImageToBinary(Skins.rollbtn)
        skn.MinimizeButtonImage = SaveImageToBinary(Skins.minbtn)
        skn.PanelButtonBG = SaveImageToBinary(Skins.panelbutton)
        skn.AppLauncherImage = SaveImageToBinary(Skins.applauncher)
        skn.TitleBarBackground = SaveImageToBinary(Skins.titlebar)
        skn.TitleLeftCornerWidth = Skins.titlebarcornerwidth
        skn.TitleRightCornerWidth = Skins.titlebarcornerwidth
        skn.ShowTitleCorners = True 'I don't know what the legacy version of this is.
        skn.TitleLeftBG = SaveImageToBinary(Skins.leftcorner)
        skn.TitleRightBG = SaveImageToBinary(Skins.rightcorner)
        skn.LeftBorderBG = SaveImageToBinary(Skins.borderleft)
        skn.RightBorderBG = SaveImageToBinary(Skins.borderright)
        skn.BottomBorderBG = SaveImageToBinary(Skins.borderbottom)
        skn.BottomLBorderBG = SaveImageToBinary(Skins.bottomleftcorner)
        skn.BottomRBorderBG = SaveImageToBinary(Skins.bottomrightcorner)

        Console.WriteLine(" ...done.")

        Console.Write("Converting desktop settings...")

        skn.AppLauncherText = Skins.applicationlaunchername
        skn.AppLauncherHolderSize = New Size(Skins.applaunchermenuholderwidth, Skins.desktoppanelheight)
        skn.AppLauncherFromLeft = New Point(0, 0)
        skn.DesktopPanelHeight = Skins.desktoppanelheight
        Select Case Skins.desktoppanelposition
            Case "Top"
                skn.DesktopPanelPosition = 0
            Case "Bottom"
                skn.DesktopPanelPosition = 1
        End Select
        skn.DesktopPanelColor = Skins.desktoppanelcolour
        skn.DesktopPanelClockBackgroundColor = Skins.clockbackgroundcolor
        skn.DesktopPanelClockColor = Skins.clocktextcolour
        skn.DesktopPanelClockFont = New Font(Skins.panelclocktextfont, Skins.panelclocktextsize, Skins.panelclocktextstyle)
        skn.PanelButtonColor = Skins.panelbuttoncolour
        skn.PanelButtonTextColor = Skins.panelbuttontextcolour
        skn.PanelButtonFont = New Font(Skins.panelbuttontextfont, Skins.panelbuttontextsize, Skins.panelbuttontextstyle)
        skn.PanelButtonHolderFromLeft = skn.AppLauncherHolderSize.Width + Skins.panelbuttoninitialgap
        skn.PanelButtonSize = New Size(Skins.panelbuttonwidth, Skins.panelbuttonheight)
        skn.PanelButtonFromLeft = New Point(Skins.panelbuttongap, Skins.panelbuttonfromtop)
        skn.DesktopPanelClockFromRight = New Point(0, 0)
        skn.DesktopColor = Skins.desktopbackgroundcolour

        Console.WriteLine(" ...done")
        Console.Write("Creating new menu color scheme from App Launcher settings...")

        skn.Menu_TextColor = Skins.applicationsbuttontextcolour
        skn.Menu_SelectedTextColor = skn.Menu_TextColor
        skn.Menu_MenuBorder = Skins.applauncherbuttoncolour
        skn.Menu_MenuItemBorder = skn.Menu_MenuBorder
        skn.Menu_ToolStripDropDownBackground = skn.Menu_MenuItemBorder
        skn.Menu_ImageMarginGradientBegin = skn.Menu_ToolStripDropDownBackground
        skn.Menu_ImageMarginGradientMiddle = skn.Menu_ToolStripDropDownBackground
        skn.Menu_ImageMarginGradientEnd = skn.Menu_ToolStripDropDownBackground
        skn.Menu_MenuStripGradientBegin = skn.Menu_ImageMarginGradientBegin
        skn.Menu_MenuStripGradientEnd = skn.Menu_ImageMarginGradientBegin
        skn.Menu_ToolStripBorder = skn.Menu_MenuBorder
        skn.Menu_CheckBackground = skn.Menu_MenuBorder
        skn.Menu_MenuItemSelected = Skins.applaunchermouseovercolour
        skn.Menu_MenuItemPressedGradientBegin = Skins.applauncherbuttonclickedcolour
        skn.Menu_MenuItemSelectedGradientBegin = skn.Menu_MenuItemSelected
        skn.Menu_MenuItemSelectedGradientEnd = skn.Menu_MenuItemSelected
        skn.Menu_MenuItemPressedGradientMiddle = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_MenuItemPressedGradientEnd = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_RaftingContainerGradientBegin = skn.Menu_MenuStripGradientBegin
        skn.Menu_RaftingContainerGradientEnd = skn.Menu_MenuStripGradientBegin
        skn.Menu_ButtonCheckedGradientBegin = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonCheckedGradientMiddle = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonCheckedGradientEnd = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonCheckedHighlightBorder = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonCheckedHighlight = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonPressedGradientBegin = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonPressedGradientMiddle = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonPressedGradientEnd = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonPressedHighlightBorder = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonPressedHighlight = skn.Menu_MenuItemPressedGradientBegin
        skn.Menu_ButtonSelectedBorder = skn.Menu_MenuItemSelected
        skn.Menu_ButtonSelectedGradientBegin = skn.Menu_MenuItemSelected
        skn.Menu_ButtonSelectedGradientMiddle = skn.Menu_MenuItemSelected
        skn.Menu_ButtonSelectedGradientEnd = skn.Menu_MenuItemSelected
        skn.Menu_ButtonSelectedHighlight = skn.Menu_MenuItemSelected
        skn.Menu_ButtonSelectedHighlightBorder = skn.Menu_MenuItemSelected
        skn.Menu_SeparatorDark = skn.Menu_TextColor
        skn.Menu_SeparatorLight = skn.Menu_TextColor
        skn.Menu_ToolStripBorder = skn.Menu_MenuBorder
        skn.Menu_ToolStripGradientBegin = skn.Menu_MenuBorder
        skn.Menu_ToolStripGradientEnd = skn.Menu_MenuBorder
        skn.Menu_ToolStripContentPanelGradientBegin = skn.Menu_MenuBorder
        skn.Menu_ToolStripContentPanelGradientEnd = skn.Menu_MenuBorder

        Console.WriteLine(" ...done")

        Console.Write("Setting up image layouts... ")

        Dim type = skn.GetType()
        For Each member As System.Reflection.FieldInfo In type.GetFields(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
            For Each attribute As Attribute In member.GetCustomAttributes(False)
                Try
                    Dim image As ImageAttribute = attribute
                    'WHY CAN'T WE HAVE A C#-LIKE Is OPERATOR, VB?
                    'I don't want to use a damn BODGE to check if my attribute is an ImageAttribute.
                    'Oh well.

                    If skn.SkinImageLayouts.ContainsKey(image.Name) Then
                        skn.SkinImageLayouts(image.Name) = Skins.GetLayout(image.Name)
                    Else
                        skn.SkinImageLayouts.Add(image.Name, Skins.GetLayout(image.Name))
                    End If

                Catch

                End Try
            Next
        Next

        Console.WriteLine(" ...done.")

        Console.Write("Converting window settings...")

        skn.TitlebarHeight = Skins.titlebarheight
        skn.TitleTextColor = Skins.titletextcolour
        skn.TitleBackgroundColor = Skins.titlebarcolour
        skn.TitleRightCornerBackground = Skins.rightcornercolour
        skn.TitleLeftCornerBackground = Skins.leftcornercolour
        skn.TitleFont = New Font(Skins.titletextfontfamily, CType(Skins.titletextfontsize, Single), CType(Skins.titletextfontstyle, FontStyle))
        skn.TitleTextLeft = New Point(Skins.titletextfromside, Skins.titletextfromtop)
        Select Case Skins.titletextpos
            Case "Left"
                skn.TitleTextCentered = False
            Case "Centre"
                skn.TitleTextCentered = True
        End Select
        skn.CloseButtonColor = Skins.closebtncolour
        skn.MinimizeButtonColor = Skins.minbtncolour
        skn.MaximizeButtonColor = Skins.rollbtncolour
        skn.CloseButtonSize = Skins.closebtnsize
        skn.MinimizeButtonSize = Skins.minbtnsize
        skn.MaximizeButtonSize = Skins.rollbtnsize
        skn.CloseButtonFromSide = New Point(Skins.closebtnfromside, Skins.closebtnfromtop)
        skn.MaximizeButtonFromSide = New Point(Skins.rollbtnfromside, Skins.rollbtnfromtop)
        skn.MinimizeButtonFromSide = New Point(Skins.minbtnfromside, Skins.minbtnfromtop)
        skn.BorderLeftBackground = Skins.borderleftcolour
        skn.BorderRightBackground = Skins.borderrightcolour
        skn.BorderBottomBackground = Skins.borderbottomcolour
        skn.BorderBottomLeftBackground = Skins.bottomleftcornercolour
        skn.BorderBottomRightBackground = Skins.bottomrightcornercolour
        skn.LeftBorderWidth = Skins.borderwidth
        skn.RightBorderWidth = Skins.borderwidth
        skn.BottomBorderWidth = Skins.borderwidth

        Console.WriteLine(" ...done")
        Console.Write("Generating defaults for unknown values...")

        skn.ControlColor = Color.White
        skn.ControlTextColor = Color.Black
        skn.MainFont = New Font("Microsoft Sans Serif", 8.25, FontStyle.Regular)
        skn.HeaderFont = New Font("Microsoft Sans Serif", 17.5, FontStyle.Regular)
        skn.Header2Font = New Font("Microsoft Sans Serif", 15, FontStyle.Regular)
        skn.Header3Font = New Font("Microsoft Sans Serif", 12.5, FontStyle.Regular)

        skn.TerminalBackColorCC = ConsoleColor.Black

        Console.WriteLine(" ...done!")
        Console.WriteLine("Skin conversion complete.")

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
