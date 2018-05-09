# Rabbitmq Seminar (in C#)


## Instructions
Prepare your work environment by following these instructions

1. _Install chocolatey_. [Chocolatey](https://chocolatey.org/) is OS package management for windows, just like apt is for ubuntu. Open a command line in **Admin mode** and paste the following:
   * `@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"`
1. _Install dotnetcore (using chocolatey)_. Close the previous command line window and open a **new Admin mode** command line window, then type in (this will take some time to complete):
   * `choco install dotnetcore-sdk -y`
1. _Git-clone this project to your workspace_. Open your workspace in a command line window and type the following command
   * `git clone https://github.com/bottlenecked/rabbitmq-seminar-csharp.git`
1. _Create you own branch_. Switch to the newly created folder, go to the git revision of the first exercise.
   * `cd rabbitmq-seminar-csharp`
   * `git checkout e0f5be8`
   * `git checkout -b rerabbit`