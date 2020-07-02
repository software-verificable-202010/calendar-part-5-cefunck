# calendar-part-4-cefunck

This is the repository corresponding to the fourth part of the project of the course "Calidad y verificación de software".

### Current Features

  - It shows a whole selected month.
  - It highlights Saturdays and Sundays with red foreground.
  - It allows the user to navigate through the months.
  - It displays the current month at start.
  - It allows the user to navigate through the weeks.
  - Is allows CRUD appointmenst in monthly view.
  - It allows save appointments persistently in monthly view.
  - Is allows login.
  


### How to run it

  - Clone the project.
  - Open the project with Visual Studio 2017 (or newer) and .NET Framework 4.6.1 (or newer).
  - Run the proyect
  - **Login with usuario1 or usuario2 as username.**
  - **Close calendar to logout.**
  - Navigate in both monthly view and weekly view with arrow buttons in navigation bar.
  
### Explanations of false positives in static analysis

  - CA1002: The classes in my project that expose generic lists are not designed for inheritance so this warning can be suppressed.
  - CA2210: The project context does not require an assembly signature so this warning can be suppressed.
  

### Tech Stack

  - [Visual studio 2017]
  - [.NET Framework 4.6.1]
  - [NUnit 3.12.0]
  - [NUnit3TestAdapter v3.16.1]
  - [OpenCover v4.7.992]
  - [AxoCover v1.1.7.0]
  - [Moq v4.14.15]
