SElect c.id, c.code 
from courses c
LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id 
Where c.parent_id IS NOT null AND c.semester_id = 1 and cdr.department_id = 1 AND c.lecture_group = 'GR1'  AND c.id IN
(SELECT c1.id 
from courses c1 where (c1.laboratory_group='Lab1' OR c1.laboratory_group IS NULL) AND (c1.numeric_group='U1' OR c1.numeric_group IS NULL)
AND c1.lecture_group=c.lecture_group);