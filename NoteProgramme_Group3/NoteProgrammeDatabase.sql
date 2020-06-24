Create database WindowProgrammingSQL
Go

Use WindowProgrammingSQL
Go

Create Table Users
(
	Username VARCHAR(30) not null primary key,
	Pass_word VARCHAR(30) not null
)

Create Table List_Of_Notes
(
	Username VARCHAR(30) not null,
	NotesOrder int not null,
	NotesHeader text,
	NotesContent text
	foreign key (Username) references Users(Username),
	primary key (Username,NotesOrder)
)

Create Table List_Of_TrashBin
(
	Username VARCHAR(30) not null,
	TrashOrder int not null,
	TrashHeader text,
	TrashContent text,
	foreign key (Username) references Users(Username),
	primary key (Username,TrashOrder)
)

Alter Table List_Of_Notes
Add FontFamily VARCHAR(30), FontSize int, NoteColor Varchar(30)
Alter Table List_Of_TrashBin
Add FontFamily VARCHAR(30), FontSize int, TrashColor Varchar(30)

Alter Table List_Of_Notes
Add NoteTag Varchar(30)
Alter Table List_Of_TrashBin
Add TrashTag Varchar(30)

Alter Table List_Of_Notes
Add  NotePictureName NVarchar(70)
Alter Table List_Of_TrashBin
Add  TrashPictureName NVarchar(70)

Alter Table List_Of_Notes
Add  NoteDrawPictureName NVarchar(70)
Alter Table List_Of_TrashBin
Add  TrashDrawPictureName NVarchar(70)

Create Table List_Of_ImportantNote
(
	Username VARCHAR(30) not null,
	ImportantOrder int not null,
	ImportantHeader text,
	ImportantContent text,
	ImportantTag Varchar(30),
	FontFamily VARCHAR(30), FontSize int, NoteColor Varchar(30),
	NotePictureName NVarchar(70), NoteDrawPictureName NVarchar(70),
	foreign key (Username) references Users(Username),
	primary key (Username,ImportantOrder)
)

Alter Table List_Of_ImportantNote
Add NoteOrder int not null

Alter Table Users
Add Email Varchar(50)

Alter Table Users
Add EPassword Varchar(30)
