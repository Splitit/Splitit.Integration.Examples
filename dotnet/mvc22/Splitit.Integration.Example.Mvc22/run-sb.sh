dotnet build
sleep 3 && google-chrome http://localhost:5000/Scenario/Basic &
dotnet run --launch-profile test