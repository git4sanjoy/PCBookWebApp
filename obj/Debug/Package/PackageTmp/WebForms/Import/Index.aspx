<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="PCBookWebApp.WebForms.Import.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Import HB Previous Data</title>
    <style type="text/css">
        /*
        * multi-line comment
        */
        p {
            line-height: 1em;
        }

        h1, h2, h3, h4 {
            color: orange;
            font-weight: normal;
            line-height: 1.1em;
            margin: 0 0 .5em 0;
        }

        h1 {
            font-size: 1.7em;
        }

        h2 {
            font-size: 1.5em;
        }

        a {
            color: black;
            text-decoration: none;
        }

            a:hover,
            a:active {
                text-decoration: underline;
            }

        /* you can structure your code's white space so that it is as readable for when you come back in the future or for other people to read and edit quickly */

        body {
            font-family: arial;
            font-size: 80%;
            line-height: 1.2em;
            width: 100%;
            margin: 0;
            background: #eee;
        }
        /* you can put your code all in one line like above */
        #page {
            margin: 20px;
        }

        /* or on different lines like below */
        #logo {
            width: 35%;
            margin-top: 5px;
            font-family: georgia;
            display: inline-block;
        }
        /* but try and be as concise as possible */
        #nav {
            width: 60%;
            display: inline-block;
            text-align: right;
            float: right;
        }

            #nav ul {
            }

                #nav ul li {
                    display: inline-block;
                    height: 62px;
                }

                    #nav ul li a {
                        padding: 20px;
                        background: orange;
                        color: white;
                    }

                        #nav ul li a:hover {
                            background-color: #ffb424;
                            box-shadow: 0px 1px 1px #666;
                        }

                        #nav ul li a:active {
                            background-color: #ff8f00;
                        }

        #content {
            margin: 30px 0;
            background: white;
            padding: 20px;
            clear: both;
        }

        #footer {
            border-bottom: 1px #ccc solid;
            margin-bottom: 10px;
        }

            #footer p {
                text-align: right;
                text-transform: uppercase;
                font-size: 80%;
                color: grey;
            }

        /* multiple styles seperated by a , */
        #content,
        ul li a {
            box-shadow: 0px 1px 1px #999;
        }
    </style>
</head>
<body>

	<div id="page">
		<div id="logo">
			<h1><a href="http://localhost:12005/WebForms/Import/Index.aspx" id="logoLink"  style="color:orange;">PC App v-1.0.1</a></h1>
		</div>
		<div id="nav">
			<ul>
				<li><a href="#/home.html">Home</a></li>
				<li><a href="#/about.html">About</a></li>
				<li><a href="#/contact.html">Contact</a></li>
			</ul>	
		</div>
		<div id="content">
			<h2>Import HB Previous Data</h2>
			<p>
				Import description This is my first webpage! I was able to code all the HTML and CSS in order to make it. Watch out world of web design here come!    
			</p>

            <form id="form1" runat="server">
                <div>
                    <asp:Label ID="lblStatus" runat="server" Font-Bold="True" Font-Size="Larger" ForeColor="#006600"></asp:Label>
                    <br />
                    <br />
                    <asp:Button ID="Button1" runat="server" Text="Memo Master Data ...." Font-Bold="True" Font-Size="Medium" ForeColor="White" OnClick="Button1_Click" BackColor="#FF9900" BorderColor="#FF9900" BorderStyle="None" Width="317px" Height="50px" />
                    <br />
                    <br />
                    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Memo Details Data ...." BackColor="#FF9900" Font-Bold="True" Font-Size="Medium" ForeColor="White" Width="316px" BorderColor="#FF9900" BorderStyle="None" Height="50px" />
                    <br />
                    <br />
                    <asp:Button ID="UpdateMemoNo" runat="server" Text="Update Memo No" Height="50px" OnClick="UpdateMemoNo_Click" Width="317px" />
                </div>
            </form>
            <!-- webpage content goes here in the body -->
		</div>
		<div id="footer">
			<p>
				Webpage made by <a href="/" target="_blank">Sanjoy Debnath.</a>
			</p>
		</div>
	</div>
</body>
</html>
