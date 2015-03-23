'Imports acme.common.fbClassLibrary
'Imports acme.common
'Imports acme.common.winlib
'Imports acme.common.winlib.db
'Imports acme.common.winlib.eMail
'Imports acme.common.winlib.CleanText
Imports System.Net.Mail
Imports winlib.db
Imports winlib.CleanText
Imports winlib.eMail
Imports winlib

Partial Class contact_Default
    Inherits Page
    Private _ticketNum As Long
    Private _newTicket As Boolean = False
    ''' <summary>
    ''' This event checks validation on fields completed by user.  Upon verification an email is created and sent so that information can be automatically
    ''' added to flightwise db customer service tables.  An insert is also done to the customerservicetickets table in the myradar db for reference (as per Andy wlc 8/6/14)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Submit_Click(sender As Object, e As EventArgs) Handles Submit.Click
        Try
            Select Case subject.Value
                Case "Subject", ""
                    subject.Style.Add("color", "#ffcc33")
                    subject.Value = "* A subject is required *"
                    Exit Sub
                Case Else
                    Select Case CheckSenderEmail(email.Value)
                        Case False
                            email.Style.Add("color", "#ffcc33")
                            email.Value = "* Please enter a valid email address *"
                            Exit Sub
                        Case Else
                            Select Case supp.Value
                                Case ""
                                    osType.CssClass = "validate"
                                    osType.Text = "* An operating system selection is required *"
                                    Exit Sub
                                Case Else
                                    Select Case message.Value
                                        Case "Message", ""
                                            message.Style.Add("color", "#ffcc33")
                                            message.Value = "* Don't forget to tell us what you need *"
                                            Exit Sub
                                        Case Else
                                            Dim _urgent As Boolean = False
                                            Select Case urgent.Value
                                                Case ""
                                                    _urgent = False
                                                Case Else
                                                    _urgent = True
                                            End Select
                                            Dim _tick As Long = CreateServiceTicket(email.Value, phone.Value, supp.Value & ":  " & subject.Value, message.Value, _urgent, Request.UserHostAddress, "MyRadar")
                                            Dim sendTo As String ' = ConfigurationManager.AppSettings("customerServiceGroup")
                                            Dim sendFrom As String = email.Value
                                            Dim sendSubject As String = supp.Value & ":  " & subject.Value
                                            Dim sendMessage As String = "Is this urgent?  :  " & _urgent & "<br />Callback Number:  " & phone.Value & "<br /><br />" & StripAll(message.Value) & "<br/><br/>My Radar db ticket number is:  " & _tick


                                            Select Case supp.Value
                                                Case "AN"
                                                    sendTo = "myradarandroid@acmeaom.com"
                                                Case Else
                                                    'sendTo = "service@flightwise.com"
                                                    sendTo = "support@myradar.com"
                                            End Select

                                            'If _tick = 0 Then
                                            '    validation.Text = "There was an issue creating your service ticket, please try again"
                                            '    validation.CssClass = "validate"
                                            'Else
                                            If sendMail(sendTo, sendFrom, sendSubject, sendMessage, "none", "none", "MyRadar") Then
                                                subject.Value = ""
                                                email.Value = ""
                                                phone.Value = ""
                                                message.Value = ""
                                                'TickSent.Text = "Your request has been sent to our Customer Service folks - please reference ticket number " & _tick & " for any future needs"
                                                TickSent.Text = "Your request has been sent to our Customer Service folks!"
                                                CustomerSupport.ActiveViewIndex = 1
                                            Else
                                                validation.Text = "There was an issue creating your service ticket, please try again"
                                            End If
                                            'End If


                                    End Select
                            End Select
                    End Select
            End Select
        Catch ex As Exception

        End Try

    End Sub

    Public Function CreateServiceTicket(ByVal emailFrom As String, ByVal callback As String, ByVal subj As String, ByVal msgbody As String, ByVal isUrgent As Boolean, Optional ByVal IP As String = "", Optional ByVal app As String = "Flightwise") As Long
        Const userid = 1
        Try
            'Dim db As New db(database.CustomerWrite)
            Dim db As New db

            db.Qry = "CreateServiceTicket"
            _ticketNum = db.RunCommandValue(Data.CommandType.StoredProcedure, "@Subject,@Callback,@EmailFrom,@User", subj & "," & callback & "," & emailFrom & "," & userid)
            'db.Qry = "CreateCustomerServiceEmail"
            'db.RunCommand(Data.CommandType.StoredProcedure, "@user,@maildate,@ticketnum,@emailfrom,@subject,@message,@HTML,@IP", userid & "," & Date.Now & "," & _ticketNum & "," & emailFrom & "," & subj & "," & msgbody & "," & "" & "," & IP)
            Select Case _ticketNum
                Case 0
                    Return 0
                Case Else
                    Return _ticketNum
            End Select
            db.Close()
        Catch ex As Exception
            Return 0
        Finally

        End Try
    End Function

    Public Shared Function sendMail(ByVal sendTo As String, ByVal From As String, ByVal Subject As String, ByVal Body As String, Optional ByVal sendCC As String = "None", Optional ByVal attachments As String = "None", Optional ByVal app As String = "Flightwise") As Boolean
        Try
            ' 'Declare Variables/Values
            Dim mm As New MailMessage()
            Const RegExExpression As String = "^\s*(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*\s*$"

            ' 'send email to desired recipient
            mm.From = New MailAddress(From)
            mm.To.Clear()
            mm.CC.Clear()

            Dim arrTo As String() = Split(sendTo, ",")
            '   'ADD ALL EMAIL ADDRESSES FROM ARRAY (arrTo)
            For i As Integer = 0 To arrTo.Length - 1
                Select Case Regex.IsMatch(arrTo(i), RegExExpression)
                    Case True
                        mm.To.Add(arrTo(i))
                End Select
            Next

            Select Case sendCC
                Case "None"
                    Exit Select
                Case Else
                    Dim arrCC As String() = Split(sendCC, ",")
                    '   'ADD ALL EMAIL ADDRESSES FROM ARRAY (arrCC)
                    For i As Integer = 0 To arrCC.Length - 1
                        Select Case Regex.IsMatch(arrCC(i), RegExExpression)
                            Case True
                                mm.CC.Add(arrCC(i))
                        End Select


                    Next
            End Select

            Select Case attachments
                Case "None"
                    Exit Select
                Case Else
                    Dim arrAttach As String() = Split(attachments, ",")
                    '   'ADD ALL EMAIL ATTACHMENTS FROM ARRAY (arrAttach)
                    For i As Integer = 0 To arrAttach.Length - 1
                        Select Case Regex.IsMatch(arrAttach(i), RegExExpression)
                            Case True
                                mm.Attachments.Add(New Attachment(arrAttach(i)))
                        End Select


                    Next
            End Select

            'Assign the MailMessage's properties

            mm.Subject = Subject '"Forwarded from MyRadar - " & Subject


            ' 'Body of email

            Body &= "<br /><br/>*** AUTOMATICALLY GENERATED - DO NOT REPLY TO THIS EMAIL DIRECTLY ***" & "<br />"
            mm.Body = Body

            mm.IsBodyHtml = True

            ' 'Create the SmtpClient object
            ' 'Send the MailMessage

            'Select Case app
            '    Case "Flightwise"
            '        Dim smtp As New SmtpClient
            '        With smtp
            '            .Host = "mail.flightwise.com"
            '            .Send(mm)
            '        End With
            '    Case "MyRadar"
            Dim smtpclient As SmtpClient = New SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587))
            Dim creds As System.Net.NetworkCredential = New System.Net.NetworkCredential("azure_0f4222d1e88665c09c6239dfe8bad4a2@azure.com", "upxnbx2e")
            smtpclient.Credentials = creds
            smtpclient.Send(mm)
            'End Select
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CustomerSupport.ActiveViewIndex = 0
    End Sub
End Class