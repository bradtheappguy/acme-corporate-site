<%@ Page Title="" Language="VB" MasterPageFile="/MyRadar.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="contact_Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="../css/ACMEmetro.css" />
    <style>
        footer {
            height: 32px;
        }

        .base-frame {
            background: url(../images/contact-bg.jpg) no-repeat center top;
        }

        input, textarea {
            width: 90%;
            background-color: #000;
            border-radius: 5px;
            border: 1px solid #585858;
            resize: none;
            /*width:95%;*/
            padding:9px 9px 9px 32px;
            color: #bdbdbd;
            font-family: "SegoeWP";
            font-weight: 700;
            font-size: 15px;
            margin-bottom: 15px;
        }
        textarea{padding:9px;width:98%;}
        .container{width:60%;background:#101010;}
        h2{
	font-size:30px;
	padding:0 0  15px;
	color:#fff;
	font-family: "SegoeWPBlack";
	-webkit-font-smoothing: antialiased !important;
	text-transform:uppercase;
}

h2 span{
	color:#ffcc33;
	font-weight: 100;
	font-family: "SegoeWP";
}
@media all and (max-width:800px) {
        .container{width:95%;background:#101010;}
         input{margin-left:0;}
}
        @media all and (max-width: 480px) {
            .container {
                width: 95%;
                background: #101010;
            }
            input{margin-left:0;}
        }
@media all and (max-width: 320px) {

     .container{width:95%;background:#101010;}
      input{margin-left:0;}
}
    </style>
    <script src="../Scripts/jquery-1.9.0.min.js"></script>
    <script>
        $(function () {

            // radio				
            $(".support-for a").click(function (event) {
                event.preventDefault(); 
                $(".support-for a").removeClass("active");
                $(".support-for").parent().find("input").val($(this).attr("title"))
                $(this).addClass("active");
                //alert($(".support-for").parent().find("input").val())				
            });

            //checkbox
            $(".urgent").click(function () {

                $(this).toggleClass("active");

                if ($(this).hasClass("active"))
                    $(".urgent input").val("Its urgent");
                else
                    $(".urgent input").val("");

            });

            //submit
            $('input[type="submit"]').click(function () {
                $(this).css('color', 'white');
                $(this).css('background-image', 'url(../images/btn-bg-click.png) repeat-x left top');
            });


            //
            $("input,textarea", ".contact-form").focus(function () {

                obj = $(this);

                if (obj.val() == obj.attr("default"));
                obj.val("");

            });

            $("input,textarea", ".contact-form").blur(function () {
                obj = $(this);

                if (obj.val() == "")
                    obj.val(obj.attr("default"));
            })

            //now start implementing validation n submition of the form
            $("#contact-form").submit(function () {
                $(".submit input").removeClass("send");
                //do the validations here

                //then send to server side via ajax
                var URL = "mailer.php"
                formData = $(this).serializeArray();

                $.ajax({
                    type: "POST",
                    cache: false,
                    dataType: "json",
                    url: URL,
                    data: formData,
                    beforeSend: function () {
                        //show  status
                    },
                    success: function (response) {
                        alert(JSON.stringify(response))
                    },
                    error: function (err) {
                        try {
                            console.log(err);
                        }
                        catch (err) {

                        }

                    }
                })

                return false;

            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="navContent" runat="Server">
    <a href="../who-we-are/default.aspx">About Us</a>
    <a href="../contact/default.aspx" class="active">Contact Us</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="Server">
  <asp:TextBox runat="server" ID="validation" CssClass="val-hidden" Text=""></asp:TextBox>
            <div class="container contact-form-wrapper">
                      
                        <h2>
                            <span>Contact</span> Support</h2>
                    <div class="row">
               
                <asp:MultiView ID="CustomerSupport" runat="server">
                    <asp:View ID="Contact" runat="server">
                       <div class="contact-form">
                            <div class="col_6 suf_6 last">
                                <input runat="server" id="subject" type="text" name="subject" class="subject left" value="Subject" default="Subject" />
                            </div>

                            <div class="row">
                                <div class="col_6">
                                    <input runat="server" id="email" type="text" name="email" class="email left" value="E-mail" default="E-mail" />
                                </div>
                                <div class="col_6 last">
                                    <input runat="server" id="phone" type="text" name="phone" class="phone left" value="Phone" default="Phone" />
                                </div>
                            </div>
                            <div class="row">
                                <asp:Label ID="osType" runat="server" CssClass="os" Text="Please select the Operating System you are currently using" />


                                <ul class="radio-custom support-for">
                                    <li><a class="iphone" title="IO" href="#"><span></span></a></li>
                                    <li><a class="android" title="AN" href="#"><span></span></a></li>
                                    <li><a class="windows" title="WP" href="#"><span></span></a></li>
                                    <li><a class="windows8" title="W8" href="#"><span></span></a></li>
                                </ul>
                                <input type="hidden" name="support-for" value="" runat="server" id="supp" />
                            </div>
                            <div class="row">
                                <textarea runat="server" id="message" name="message" rows="5" default="Message" class="margin_top_15">Message</textarea>
                            </div>
   </div>
                            <div class="row">
                                <div class="col_3 checkbox-custom-2 urgent">
                                    <input type="hidden" name="urgent" runat="server" id="urgent" />
                                    This is urgent 
                                </div>
                                <div class="pre_5 col_3 last">
                                    <asp:Button ID="Submit" runat="server" Text="Send" CssClass="send" />
                                </div>
                            </div>
                    
                    </asp:View>
                    <asp:View ID="Confirmation" runat="server">
                        <asp:Label ID="TickSent" runat="server" CssClass="csvalidate"></asp:Label>
                    </asp:View>
                </asp:MultiView>
      </div>
            </div>
     
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footerContent" runat="Server">
</asp:Content>