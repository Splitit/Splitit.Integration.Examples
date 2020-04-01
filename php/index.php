<?php 
    $config = include 'config.php';
?>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>FlexFields example in PHP</title>

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/css/site.css" />
</head>
<body>
    <nav class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a asp-area="" asp-controller="Home" asp-action="Index" class="navbar-brand">FlexFields example in PHP</a>
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li><a asp-area="" asp-controller="Home" asp-action="Index">Home</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <div class="container body-content">

        <div class="text-left">
            <h1 class="display-4">Welcome</h1>
            <div data-splitit="true"
                data-splitit-amount="100"
                data-splitit-num-installments="3"
                data-splitit-type="banner-top">
            </div>
            <div class="row" style="margin-top: 50px;">
                <div class="col-md-2">
                    <img data-splitit-placeholder="banner" data-splitit-banner="color:shop-today-pay-over-time" width="160" />
                </div>
                <div class="col-md-8" style="padding-left: 20px;">
                    <div class="splitit-default-ui grouped">
                        <div class="splitit-cc-group">
                            <div id="card-number"></div>
                            <div id="expiration-date"></div>
                            <div id="cvv"></div>
                            <div class="splitit-cc-group-separator"></div>
                        </div>

                        <div id="installment-picker"></div>
                        <div id="error-box"></div>
                        <div id="terms-conditions"></div>
                        <button id="btn-pay"></button>
                    </div>
                </div>
                <div class="col-md-2">
                    <img data-splitit-placeholder="banner" data-splitit-banner="color:buy-now-pay-later" width="160" />
                </div>
            </div>
            
        </div>

        <hr />
        <footer>
            <p>&copy; 2020 - Splitit</p>
        </footer>
    </div>

    <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-3.3.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

    <link rel="stylesheet" type="text/css"
          href="https://hosted.sandbox.splitit.com/css/splitit.flex-fields.min.css?v=<?=round(microtime(true)/100)?>">
    <script src="https://hosted.sandbox.splitit.com/js/dist/splitit.flex-fields.sdk.js?v=<?=round(microtime(true)/100)?>">
    </script>
    
    <script>
        (function (i, s, o, g, r, a, m) {
        i['SplititObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//upstream.sandbox.splitit.com/v1/dist/upstream-messaging.js?v=' + (Math.ceil(new Date().getTime() / 100000)), 'splitit');
        splitit('init', { merchantid: '<?=$config["merchantId"]?>', lang: 'en', currency: 'USD', currencySymbol: '$', debug: false });
    </script>

    <script>
    var flexFieldsInstance = Splitit.FlexFields.setup({
        fields: {
            number: {
                selector: "#card-number"
            },
            cvv: {
                selector: "#cvv"
            },
            expirationDate: {
                selector: "#expiration-date"
            }
        },
        installmentPicker: {
            selector: "#installment-picker"
        },
        termsConditions: {
            selector: "#terms-conditions"
        },
        errorBox: {
            selector: "#error-box"
        },
        paymentButton: {
            selector: "#btn-pay"
        }
    }).ready(function () {
        var splititFlexFields = this;
        $.ajax({
            url: '/server-side.php',
            method: "post",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({
                amount: 480,
                numInstallments: null,
                billingAddress: {
                    addressLine: "260 Madison Avenue.",
                    addressLine2: "Appartment 1",
                    city: "New York",
                    state: "NY",
                    country: "USA",
                    zip: "10016"
                },
                consumerModel: {
                    fullName: "John Smith",
                    email: "JohnS@splitit.com",
                    phoneNumber: "1-844-775-4848",
                    cultureName: "en-us"
                }
            }),
            success: function (data) {
                splititFlexFields.set(data);
            }
        });

    }).onSuccess(function (result) {
        // Respond here if everything goes well.
        alert('Payment was successful! Check console for result details.');
        console.log(result);
    });

    </script>
</body>
</html>




