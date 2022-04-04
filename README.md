# Antivirus

Antivirus model developed by me during my internship at the Institute of Computer Technology

Detects in real-time malware such as virus signatures, code caves, and system DLL spoofing. 
Has a simulation of a remote real-time database of virus signatures (used EICAR signature as an example)

**Technology stack used:**
C#, C++, .NET Framework, WPF, Win32 API, MS-SQL (for system DLLs unautorized changes tracking), Firebase (to imitate some remote virus signature), encryption and decryption systems (MD5, SHA1)

**Examples:**
_**1. Main window**_

![image](https://user-images.githubusercontent.com/19497575/161505154-99b740d7-0c4c-4ff8-97d4-3f3250cf1cac.png)

_**2. Malware detected:**_

![image](https://user-images.githubusercontent.com/19497575/161505346-b93f07a4-3b7a-4349-aa6e-938b79f1dda0.png)

_**3. Choose a directory to scan:**_

![image](https://user-images.githubusercontent.com/19497575/161505406-2b1a4492-4f63-4d86-9480-3d0b4c980c12.png)

_**4. Directory scanning:**_

![image](https://user-images.githubusercontent.com/19497575/161505531-7e8979f5-6616-4d29-92ba-73405df81932.png)

_**5. Remote virus signatures database (Firebase) simulation structure:**_

![image](https://user-images.githubusercontent.com/19497575/161505780-51b00a7d-45fe-42bf-b1e1-dde93e063b3e.png)

**Project structure:**

![image](https://user-images.githubusercontent.com/19497575/161505871-b4d16402-3950-4d0f-9d7d-f03bddf8c38b.png)

**Auto-generated class diagram:**

![image](https://user-images.githubusercontent.com/19497575/161505929-db07c35e-2999-4722-a7ba-551aa75dd444.png)

