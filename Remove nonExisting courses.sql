
DELETE s from timetable_spring s
LEFT JOIN courses c on c.code = s.course_code
Where c.id IS NULL;