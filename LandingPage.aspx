<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LandingPage.aspx.cs" Inherits="As200537F.LandingPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Helper/css/style.default.css" rel="stylesheet" />
    <link href="Helper/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="header">
            <nav class="navbar navbar-expand-lg px-4 py-4 bg-white shadow">
            </nav>
        </header>

        <div class="page-holder w-100 d-flex flex-wrap">
            <div class="container-fluid px-x1-5">
                <section class="py-5">
                    <div class="row">

                        <div class="col-lg-4">
                            <div class="card">
                                <div class="card-header">
                                    <h3 class="h6 text-uppercase mb-0">Already have an account login here</h3>
                                </div>
                                <div class="card-body">
                                    <a href="Login.aspx" type="button" class="btn btn-primary">Login</a>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4">
                            <div class="card">
                                <div class="card-header">
                                    <h3 class="h6 text-uppercase mb-0">Don't have an account Sign up here</h3>
                                </div>
                                <div class="card-body">
                                    <a href="Signup.aspx" type="button" class="btn btn-primary">Sign up</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

            </div>

        </div>
    </form>
</body>
</html>

