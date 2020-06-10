dotnet build
sleep 3 && google-chrome http://localhost:5000/Scenario/EmbeddedPaymentForm &
dotnet run --launch-profile staging