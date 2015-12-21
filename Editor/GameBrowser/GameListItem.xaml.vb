﻿Imports System.Windows
Imports System.Windows.Media

Public Class GameListItem
    Public Enum State
        ReadyToPlay
        NotDownloaded
        Downloading
    End Enum

    Public Enum ContextMenuTypes
        Recent
        Download
    End Enum

    Public Event Launch(filename As String)
    Public Event ClearAllItems()
    Public Event RemoveItem(sender As GameListItem, filename As String)
    Public Event Clicked(sender As GameListItem)
    Public Event StateChanged()

    Private m_filename As String
    Private m_url As String
    Private m_downloading As Boolean = False
    Private WithEvents m_client As System.Net.WebClient
    Private WithEvents m_imageClient As System.Net.WebClient
    Private m_downloadFilename As String
    Private m_setDownloadTooltip As Boolean
    Private m_isOnlineItem As Boolean
    Private m_gameId As String
    Private m_data As GameListItemData
    Private m_state As State = State.ReadyToPlay
    Private m_playButtonWidth As Integer
    Private m_playButtonRight As Integer
    Private m_hoverBrush As Brush
    Private m_background As Brush
    Private m_selectedBackground As Brush
    Private m_isSelected As Boolean
    Private m_contextMenuType As ContextMenuTypes

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        SetToolTipText("")
        ContextMenuType = ContextMenuTypes.Recent
        ratingBlock.Visibility = Windows.Visibility.Collapsed
        notRatedBlock.Visibility = Windows.Visibility.Collapsed

        m_hoverBrush = New SolidColorBrush(Color.FromRgb(&HBE, &HE6, &HFD))

        m_selectedBackground = SystemColors.HighlightBrush

        AddHandler Me.MouseEnter, AddressOf MouseEnterUpdateBackground
        AddHandler Me.MouseLeave, AddressOf MouseLeaveUpdateBackground
    End Sub

    Public Property CurrentState As State
        Get
            Return m_state
        End Get
        Set(value As State)
            Dim changed As Boolean = m_state <> value

            m_state = value

            Select Case m_state
                Case State.ReadyToPlay
                    cmdLaunch.Content = "Play"
                    info.Text = "Download complete"
                    mnuDelete.IsEnabled = True
                Case State.NotDownloaded
                    cmdLaunch.Content = "Download"
                    info.Text = "Not downloaded"
                    mnuDelete.IsEnabled = False
                Case State.Downloading
                    cmdLaunch.Content = "Cancel"
                    info.Text = "Downloading..."
                    mnuDelete.IsEnabled = False
            End Select

            progressBar.Visibility = If(m_state = State.Downloading, Windows.Visibility.Visible, Windows.Visibility.Collapsed)

            If changed Then RaiseEvent StateChanged()
        End Set
    End Property

    Public Property GameName() As String
        Get
            Return title.Text
        End Get
        Set(value As String)
            title.Text = value
        End Set
    End Property

    Public Property GameInfo() As String
        Get
            Return info.Text
        End Get
        Set(value As String)
            info.Text = value
        End Set
    End Property

    Public Property LaunchCaption() As String
        Get
            Return DirectCast(cmdLaunch.Content, String)
        End Get
        Set(value As String)
            cmdLaunch.Content = value
        End Set
    End Property

    Public Property URL As String
        Get
            Return m_url
        End Get
        Set(value As String)
            m_url = value
        End Set
    End Property

    Public Property DownloadFilename As String
        Get
            Return m_downloadFilename
        End Get
        Set(value As String)
            m_downloadFilename = value
        End Set
    End Property

    Private m_author As String

    Public Property Author As String
        Get
            Return m_author
        End Get
        Set(value As String)
            m_author = value
            If (value.Length > 0) Then
                authorLabel.Text = "by " + value
            Else
                authorLabel.Text = ""
            End If
        End Set
    End Property

    Public Property Filename As String
        Get
            Return m_filename
        End Get
        Set(value As String)
            m_filename = value
            SetToolTipText(m_filename)
        End Set
    End Property

    Public Property GameId As String
        Get
            Return m_gameId
        End Get
        Set(value As String)
            m_gameId = value
        End Set
    End Property

    Private Sub SetToolTipText(text As String)
        If text.Length = 0 Then
            Windows.Controls.ToolTipService.SetIsEnabled(textBlock, False)
        Else
            Windows.Controls.ToolTipService.SetIsEnabled(textBlock, True)
            textTooltip.Content = text
            infoTooltip.Content = text
        End If
    End Sub

    Private Sub cmdLaunch_Click(sender As System.Object, e As System.EventArgs) Handles cmdLaunch.Click
        LaunchButtonClick()
    End Sub

    Public Sub LaunchButtonClick()
        RaiseEvent Launch(m_filename)
    End Sub

    Public Property ContextMenuType As ContextMenuTypes
        Get
            Return m_contextMenuType
        End Get
        Set(value As ContextMenuTypes)
            m_contextMenuType = value
            If value = ContextMenuTypes.Download Then
                mnuDelete.Visibility = Windows.Visibility.Visible
                mnuRemove.Visibility = Windows.Visibility.Collapsed
                mnuClear.Visibility = Windows.Visibility.Collapsed
            Else
                mnuDelete.Visibility = Windows.Visibility.Collapsed
                mnuRemove.Visibility = Windows.Visibility.Visible
                mnuClear.Visibility = Windows.Visibility.Visible
            End If
        End Set
    End Property

    Private Sub mnuClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        RaiseEvent ClearAllItems()
    End Sub

    Private Sub mnuRemove_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        RaiseEvent RemoveItem(Me, m_filename)
    End Sub

    Private m_rating As Double

    Public Property Rating As Double
        Get
            Return m_rating
        End Get
        Set(value As Double)
            m_rating = value
            If value > 0 Then
                Helper.OutputStars(stars, CInt(value))
                ratingValue.Text = String.Format("({0:F1} stars)", value)
                ratingBlock.Visibility = Windows.Visibility.Visible
                notRatedBlock.Visibility = Windows.Visibility.Collapsed
            Else
                stars.Text = ""
                ratingValue.Text = ""
                ratingBlock.Visibility = Windows.Visibility.Collapsed
                notRatedBlock.Visibility = Windows.Visibility.Visible
            End If
        End Set
    End Property

    Public Sub SetBackground(background As Brush)
        m_background = background
        Me.Background = background
    End Sub

    Private Sub MouseEnterUpdateBackground(sender As Object, e As System.Windows.Input.MouseEventArgs)
        If IsSelected Then Return
        Me.Background = m_hoverBrush
    End Sub

    Private Sub MouseLeaveUpdateBackground(sender As Object, e As System.Windows.Input.MouseEventArgs)
        If IsSelected Then Return
        Me.Background = m_background
    End Sub

    Private Sub grid_MouseUp(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles grid.MouseUp
        If e.ChangedButton = Input.MouseButton.Left Then
            RaiseEvent Clicked(Me)
        End If
    End Sub

    Public Property IsOnlineItem As Boolean
        Get
            Return m_isOnlineItem
        End Get
        Set(value As Boolean)
            m_isOnlineItem = value
        End Set
    End Property

    Public Property Data As GameListItemData
        Get
            Return m_data
        End Get
        Set(value As GameListItemData)
            m_data = value
        End Set
    End Property

    Public Property IsSelected As Boolean
        Get
            Return m_isSelected
        End Get
        Set(value As Boolean)
            m_isSelected = value
            If m_isSelected Then
                Me.Background = m_selectedBackground
                Me.Foreground = SystemColors.HighlightTextBrush
            Else
                Me.Background = m_background
                Me.Foreground = SystemColors.ControlTextBrush
            End If
        End Set
    End Property

    Private Sub mnuDelete_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If System.IO.File.Exists(Filename) Then
            System.IO.File.Delete(Filename)
        End If
        CurrentState = State.NotDownloaded
    End Sub
End Class
