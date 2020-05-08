dotnet build
sleep 3 && google-chrome http://localhost:5000/Home/Debug &
dotnet run --launch-profile dev