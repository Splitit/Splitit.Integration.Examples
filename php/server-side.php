<?php 
    if ($_SERVER['REQUEST_METHOD'] != 'POST') {
        exit;
    }

    use SplititSdkClient\Configuration;
    use SplititSdkClient\ObjectSerializer;
    use SplititSdkClient\FlexFields;
    use SplititSdkClient\Api\InstallmentPlanApi;
    use SplititSdkClient\Api\LoginApi;
    use SplititSdkClient\Model\LoginRequest;
    use SplititSdkClient\Model\PlanData;
    use SplititSdkClient\Model\ConsumerData;
    use SplititSdkClient\Model\RequestHeader;
    use SplititSdkClient\Model\RedirectUrls;
    use SplititSdkClient\Model\AddressData;
    use SplititSdkClient\Model\PlanApprovalEvidence;
    use SplititSdkClient\Model\CardData;
    use SplititSdkClient\Model\MoneyWithCurrencyCode;
    use SplititSdkClient\Model\InitiateInstallmentPlanRequest;
    use SplititSdkClient\Model\CreateInstallmentPlanRequest;

    require_once(__DIR__ . '/vendor/autoload.php');
    
    header('Content-type: application/json');
    $config = include 'config.php';

    Configuration::sandbox()->setApiKey($config['apiKey']);

    $inputJSON = file_get_contents('php://input');
    $postData = json_decode($inputJSON, TRUE); //convert JSON into array
    $loginApi = new LoginApi(Configuration::sandbox());

    try{
        $request = new LoginRequest();
        
        # Replace with your login information
        $request->setUserName($config['username']);
        $request->setPassword($config['password']);
        $loginResponse = $loginApi->loginPost($request);
        
        $session_id = $loginResponse->getSessionId();
        
        $installmentPlanApi = new InstallmentPlanApi(
            Configuration::sandbox(),
            $session_id
        );

        $initiateRequest = new InitiateInstallmentPlanRequest();
       
        $plan_data = new PlanData();
        $plan_data->setNumberOfInstallments($postData['numInstallments']);
        $plan_data->setAmount(new MoneyWithCurrencyCode(array("value" => $postData['amount'], "currency_code" => "USD")));
        $initiateRequest->setPlanData($plan_data);

        $initiateRequest->setBillingAddress(new AddressData(array(
            "address_line" => $postData['billingAddress']['addressLine'],
            "address_line2" => $postData['billingAddress']['addressLine2'],
            "city" => $postData['billingAddress']['city'],
            "country" => $postData['billingAddress']['country'],
            "zip" => $postData['billingAddress']['zip']
        )));

        $initiateRequest->setConsumerData(new ConsumerData(array(
            "full_name" => $postData['consumerModel']['fullName'],
            "email" => $postData['consumerModel']['email'],
            "phone_number" => $postData['consumerModel']['phoneNumber'],
            "culture_name" => $postData['consumerModel']['cultureName']
        )));

        $wizard_data = new PaymentWizardData();
        $wizard_data->setRequestedNumberOfInstallments("1,2,3,6,9,12");
        $initiateRequest->setPaymentWizardData($wizard_data);
        
        $initResponse = $installmentPlanApi->installmentPlanInitiate($initiateRequest);

        echo $initResponse;

    } catch(Exception $e){
        echo $e;
    }
?>