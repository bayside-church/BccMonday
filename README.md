### Project Prerequisites

- Configure a local instance of RockRMS ( [instructions](https://www.notion.so/baysidechurch/Configure-local-RockRMS-a665fde12b7f49a99c49c0cb437e8e8a) )
- Navigate to **Rock > RockWeb > Plugins** and create a folder named `com_baysideonline`


---

### Setting Up the Project

- Go to BccMonday's GitHub repository ( [here](https://github.com/bayside-church/BccMonday) )
- Click on '**Code > Open with GitHub Desktop**'
- Clone the project into the root folder of '**Rock**'
  - **Desktop > Rock**
- You will now have a folder called '**BccMonday**' with all the required assets

> Note that you should not commit any changes to the **Rock** repository or any branch of the **Rock** repository. Only commit changes to the project repository you are working on.

- Using the file explorer, navigate to the '**BccMonday**' folder.
  - **Desktop > Rock > BccMonday**
- Copy the folder named '**BccMondayUI**'
- Paste the folder into the '**com_baysideonline**' folder
  - **Rock > RockWeb > Plugins > com_baysideonline**
- Open the '**Rock**' solution file in Visual Studio 2022
- In the '**Solution Explorer**', right click the solution and select '**Add > Existing Project**'
- A file explorer will appear, navigate into the '**BccMonday**' folder and select `BccMonday.csproj`
- In the '**Solution Explorer**', right click the '**BccMonday**' project and select '**Build**' *(wait until the build completes)*
- In the '**Solution Explorer**', right click the '**com_baysideonline**' folder and select '**Add Reference**'
  - A window will appear, select '**Projects > Solution**' on the left side of the window
  - Select the '**BccMonday**' checkbox, then click '**OK**'


---

### Running the Project

- Open the '**Rock**' solution file in Visual Studio 2022
- In the '**Solution Explorer**', right click the '**BccMonday**' project and select '**Build**' *(wait until the build completes)*
- On the top menu, select '**Build > Build Solution**' *(wait until the build completes)*
- Click '**IIS Express**' to run the solution file

