# Antivirus

Antivirus model developed by me during my internship at the Institute of Computer Technology

Detects in real-time malware such as virus signatures, code caves, and system DLL spoofing. 
Has a simulation of a remote real-time database of virus signatures (used EICAR signature as an example)

**Technology stack used:**
C#, C++, .NET Framework, WPF, Win32 API, MS-SQL (for system DLLs unautorized changes tracking), Firebase (to imitate some remote virus signature), encryption and decryption systems (MD5, SHA1)

**Examples:**
_**1. Main window (here you can start or stop real-time protection, or start to scan separate file or directory):**_

![image](https://user-images.githubusercontent.com/19497575/161505154-99b740d7-0c4c-4ff8-97d4-3f3250cf1cac.png)

_**2. Malware detected (here you can delete the infected file and end processes associated with it, add it to exclusions, skip this file, update its checksum, or end the scan):**_

![image](https://user-images.githubusercontent.com/19497575/161505346-b93f07a4-3b7a-4349-aa6e-938b79f1dda0.png)

_**3. Choose separate file or directory to scan (you can allow or deny checksum verification and access to the remote database of virus signatures):**_

![image](https://user-images.githubusercontent.com/19497575/161509394-9c88893a-45e9-491e-af1b-ed7c759f575b.png)

![image](https://user-images.githubusercontent.com/19497575/161505406-2b1a4492-4f63-4d86-9480-3d0b4c980c12.png)

_**4. Directory scanning:**_

![image](https://user-images.githubusercontent.com/19497575/161505531-7e8979f5-6616-4d29-92ba-73405df81932.png)

_**5. Settings window (here you can edit the list of trusted processes, the list of files checksum, and allow or deny accsess to the remote database:**_

![image](https://user-images.githubusercontent.com/19497575/161508624-b79fc8b6-d024-40c4-961d-cfa1c667283f.png)

_**6. The list of files checksums (here you can add, edit or clear the list):**_

![image](https://user-images.githubusercontent.com/19497575/161507611-1166ce98-7c21-4aed-ab10-ef0226489015.png)
![image](https://user-images.githubusercontent.com/19497575/161508284-476d079d-b553-4af5-8397-0aac7c45a485.png)

_**7. Remote virus signatures database (Firebase) simulation structure:**_

![image](https://user-images.githubusercontent.com/19497575/161505780-51b00a7d-45fe-42bf-b1e1-dde93e063b3e.png)

**Project structure:**

![image](https://user-images.githubusercontent.com/19497575/161505871-b4d16402-3950-4d0f-9d7d-f03bddf8c38b.png)

**Auto-generated class diagram:**

![image](https://user-images.githubusercontent.com/19497575/161505929-db07c35e-2999-4722-a7ba-551aa75dd444.png)

