This systems' purpose is to provide users the posibility to manage Curriculum Based Timetabling for higher education.

The system consist of these main parts:
	1.	Data Pre-processing:
		•	General information: is the configuration module where user can define the responsible person of timetable, number of working days, number of perionds per day. These data will influence the becoming parts of this system.
		•	Study programs: enables users to add, edit and delete study programs. A study program corresponds to a faculty.
		•	Teachers: consist of the module where user can add, edit and delete a teacher by using this system. To register a new teacher, user should fill some basic information (name, surname, email, phone etc.) and teacher preferences if needed. Teacher preferences are weekly based and define the periods when a teacher is not available for teaching. The maximum number of unavailable periods is the half of periods pre-defined in General Information module. These preferences on this system are treated as hard constraints ( if any of these preferences is not fulfilled , the schedule is not valid ).
		•	Rooms: a module that enables user can add, edit and delete a room where classes could be scheduled. During the proces of room registration, user has to define the building where this room is located, capacity, address etc. Room size (capacity) on this system is treated as soft constraint ( if the class has more students than the room size, it will add penalty to the overall solution, but will not make it invalid ).
		•	Courses: is the module where user can add, edit and delete courses. On this module user can register the course by defining some of basic course information like name, ects, semester, study degree, number of students, number of lecture groups, number of numeric exercises groups, number of laboratory exercises groups etc. A course should belong to at least one study program. User can register a course that belongs to all study programs. When registering a course, user can define teachers that teach for that course, rooms where that course could be held. The relation between a course and a teacher can be handled also from Course Teacher Relation module and Course Study Programs Relation module. 
		•	Timetable: is the module for schedule generation. User can find here all assigned and unasigned classes, and filter them by many filter options.  

	2.	Schedule generation:
		•	Instance generation: is implemented regarding to Udine benchmark instances and it will be used as input for automatic and half automatic scheduling.  
		•	Automatic schedule generation: is done by a library of schedule generation, implemented to be used by this system. The schedule is geterated using Iterated Local Search.
		•	Half automatic schedule generation: if user schedules some classes and wants to schedule the remaining part of timetable without affecting the already scheduled classes, the library mentioned above offers the posibility to send as input those scheduled classes and it will schedule the remaining part.

	3.	Data Post-processing:
		•	Manual lecture scheduling: user can schedule a class by using the timetable window where system proposes all best positions to schedule this class. Also will provide information for every period ( how much will wait students, if the room does not have enough seats for that class, when room or teacher is busy, when students are busy etc. )
		•	Remove assignment: wrong assigned clases can be removed from the Timetable module by selecting the class and then clicking Remove Assign button.
		•	Export to Word: when all changes are procesed, user can export timetable to Word. Export can done for all semester, for a study program, for a specific semester etc.
	
The system is implemented on C# by using Visual Studio 2019 with Blend as editor.	
