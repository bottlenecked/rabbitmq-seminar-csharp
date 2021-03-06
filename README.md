# Rabbitmq Seminar (in C#)


## Instructions

### Work environment
Prepare your work environment by following these instructions

1. _Install chocolatey_. [Chocolatey](https://chocolatey.org/) is OS package management for Windows, just like apt is for ubuntu. Open a command line in **Admin mode** and paste the following:
   * `@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"`
1. _Install dotnetcore_. Close the previous command line window and open a **new Admin mode** command line window, then type in (this will take some time to complete):
   * `choco install dotnetcore-sdk -y`
1. _Install local rabbitmq server_. Run the following command to install the rabbitmq server service and the management plugin
   * `choco install rabbitmq -y`
1. _Git-clone this project to your workspace_. **cd** to your workspace in a command line window and type the following command
   * `git clone https://github.com/bottlenecked/rabbitmq-seminar-csharp.git`
1. _Create you own branch_. Switch to the newly created folder and create a branch out of the commit for the first exercise.
   * `cd rabbitmq-seminar-csharp`
   * `git checkout -b myrabbit 1a1ce84`

### How to proceed
Look at the commit history. You will see pairs of commits named "Exercise: blahblah" and "Soultion: blahblah".
Use `git checkout [commit-sha]` to checkout the "Exercise" commits and work on the solutions.
When you are done, you can checkout the solution (perhaps by commiting your own and then comparing the diff).

Remember to use rabbitmq admin to check that queues and exchanges are created/deleted as expected, and that there are no unexpected messages left in the queues.