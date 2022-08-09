Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.Editors.Settings
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Windows.Threading
Imports DevExpress.Xpf.Ribbon

Namespace RibbonControl_Ex

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits DXRibbonWindow

        Public Sub New()
            Me.InitializeComponent()
            Me.bFileName.Content = "Document 1"
            CType(Me.eFontSize.EditSettings, ComboBoxEditSettings).ItemsSource =(New FontSizes()).Items
            InitializeFontFamilyGallery()
        End Sub

        Private Sub InitializeFontFamilyGallery()
            For Each fontFamily As FontFamily In(New DecimatedFontFamilies()).Items
                Dim src As ImageSource = CreateImage(fontFamily)
                Me.FontFamilyGalleryGroup.Items.Add(CreateItem(fontFamily, src))
                Me.FontFamilyDropDownGalleryGroup.Items.Add(CreateItem(fontFamily, src))
            Next
        End Sub

        Private fmtText As FormattedText = Nothing

        Private Function createFormattedText(ByVal fontFamily As FontFamily) As FormattedText
            Return New FormattedText("Aa", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, New Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), 18, Brushes.Black, Nothing, TextFormattingMode.Ideal)
        End Function

        Private Function CreateImage(ByVal fontFamily As FontFamily) As ImageSource
            Const DimensionSize As Double = 32
            Const HalfDimensionSize As Double = DimensionSize / 2R
            Dim v As DrawingVisual = New DrawingVisual()
            Dim c As DrawingContext = v.RenderOpen()
            c.DrawRectangle(Brushes.White, Nothing, New Rect(0, 0, DimensionSize, DimensionSize))
            If fmtText Is Nothing Then fmtText = createFormattedText(fontFamily)
            fmtText.SetFontFamily(fontFamily)
            fmtText.TextAlignment = TextAlignment.Center
            Dim verticalOffset As Double =(DimensionSize - fmtText.Baseline) / 2R
            c.DrawText(fmtText, New Point(HalfDimensionSize, verticalOffset))
            c.Close()
            Dim rtb As RenderTargetBitmap = New RenderTargetBitmap(CInt(DimensionSize), CInt(DimensionSize), 96, 96, PixelFormats.Pbgra32)
            rtb.Render(v)
            Return rtb
        End Function

        Private Function CreateItem(ByVal fontFamily As FontFamily, ByVal image As ImageSource) As GalleryItem
            Dim item As GalleryItem = New GalleryItem()
            item.Glyph = image
            item.Caption = fontFamily.ToString()
            item.Tag = fontFamily
            Return item
        End Function

        Private Sub textEditor_SelectionChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ShowHideSelectionCategory()
            UpdateStatusCaretPosition()
            InvokeUpdateFormat()
        End Sub

        Private isInvokePending As Boolean = False

        Private Sub InvokeUpdateFormat()
            If Not isInvokePending Then
                Dispatcher.BeginInvoke(DispatcherPriority.Background, New Action(AddressOf UpdateFormat))
                isInvokePending = True
            End If

            UpdateFormat()
        End Sub

        Protected Sub UpdateFormat()
            Dim value As Object = Me.textEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty)
            Me.eFontSize.EditValue = If(value Is DependencyProperty.UnsetValue, Nothing, value)
        End Sub

        Private Sub ShowHideSelectionCategory()
            If Me.textEditor Is Nothing Then
                Me.categorySelection.IsVisible = False
            Else
                Me.categorySelection.IsVisible = Not SelectedTextIsNullOrEmpty
            End If

            If Me.categorySelection.IsVisible Then Me.RibbonControl.SelectedPage = Me.categorySelection.Pages(0)
        End Sub

        Public ReadOnly Property SelectedTextIsNullOrEmpty As Boolean
            Get
                Return String.IsNullOrEmpty(Me.textEditor.Selection.Text)
            End Get
        End Property

        Private Sub UpdateStatusCaretPosition()
            Dim line As Integer = 0
            Me.textEditor.CaretPosition.GetLineStartPosition(-100000, line)
            Dim col As Integer = Me.textEditor.CaretPosition.GetOffsetToPosition(Me.textEditor.CaretPosition.GetLineStartPosition(0))
            Me.bPosInfo.Content = "Line: " & (-line).ToString() & "  Position: " & (-col).ToString()
        End Sub

        Private Sub FontFamilyGallery_ItemChecked(ByVal sender As Object, ByVal e As GalleryItemEventArgs)
            Dim newFontFamily As FontFamily = CType(e.Item.Tag, FontFamily)
            ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, newFontFamily)
        End Sub

        Private Sub eFontSize_EditValueChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, Me.eFontSize.EditValue)
        End Sub

        Private Sub ApplyPropertyValueToSelectedText(ByVal formattingProperty As DependencyProperty, ByVal value As Object)
            If value Is Nothing Then Return
            Me.textEditor.Selection.ApplyPropertyValue(formattingProperty, value)
        'InvokeUpdateFormat();
        'InvokeFocusEdit();
        End Sub

        Private Sub OptionsButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            TryCast(Me.RibbonControl.ApplicationMenu, ApplicationMenu).ClosePopup()
        End Sub

        Private Sub ExitButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            TryCast(Me.RibbonControl.ApplicationMenu, ApplicationMenu).ClosePopup()
        End Sub

        Private Sub groupEdit_CaptionButtonClick(ByVal sender As Object, ByVal e As EventArgs)
            MessageBox.Show("DevExpress Ribbon Control", "Edit Settings Dialog")
        End Sub

        Private Sub bAbout_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
            MessageBox.Show("DevExpress Ribbon Control", "About Window")
        End Sub

        Private Sub groupFile_CaptionButtonClick(ByVal sender As Object, ByVal e As RibbonCaptionButtonClickEventArgs)
            MessageBox.Show("DevExpress Ribbon Control", "File Settings Dialog")
        End Sub
    End Class

    Public Class RecentItem

        Public Property Number As Integer

        Public Property FileName As String
    End Class

    Public Class ButtonWithImageContent

        Public Property ImageSource As String

        Public Property Content As Object
    End Class

    Public Class FontSizes

        Public ReadOnly Property Items As Double()
            Get
                Return New Double() {3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0}
            End Get
        End Property
    End Class

    Public Class DecimatedFontFamilies
        Inherits FontFamilies

        Const DecimationFactor As Integer = 5

        Public Overrides ReadOnly Property Items As ObservableCollection(Of FontFamily)
            Get
                Dim res As ObservableCollection(Of FontFamily) = New ObservableCollection(Of FontFamily)()
                For i As Integer = 0 To ItemsCore.Count - 1
                    If i Mod DecimationFactor = 0 Then res.Add(ItemsCore(i))
                Next

                Return res
            End Get
        End Property
    End Class

    Public Class FontFamilies

        Private Shared itemsField As ObservableCollection(Of FontFamily)

        Protected Shared ReadOnly Property ItemsCore As ObservableCollection(Of FontFamily)
            Get
                If itemsField Is Nothing Then
                    itemsField = New ObservableCollection(Of FontFamily)()
                    For Each fam As FontFamily In Fonts.SystemFontFamilies
                        If Not IsValidFamily(fam) Then Continue For
                        itemsField.Add(fam)
                    Next
                End If

                Return itemsField
            End Get
        End Property

        Public Shared Function IsValidFamily(ByVal fam As FontFamily) As Boolean
            For Each f As Typeface In fam.GetTypefaces()
                Dim g As GlyphTypeface = Nothing
                Try
                    If f.TryGetGlyphTypeface(g) Then
                        If g.Symbol Then Return False
                    End If
                Catch __unusedException1__ As Exception
                    Return False
                End Try
            Next

            Return True
        End Function

        Public Overridable ReadOnly Property Items As ObservableCollection(Of FontFamily)
            Get
                Dim res As ObservableCollection(Of FontFamily) = New ObservableCollection(Of FontFamily)()
                For Each fm As FontFamily In ItemsCore
                    res.Add(fm)
                Next

                Return res
            End Get
        End Property
    End Class
End Namespace
