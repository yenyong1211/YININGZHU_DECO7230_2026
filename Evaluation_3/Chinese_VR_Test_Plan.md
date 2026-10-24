# Chinese VR Learning Prototype Test Plan

*IP2b user testing and functional checks*

## 1 Purpose and scope

This study evaluates whether the current controller-based prototype is understandable, stable and usable, and explores immediate understanding of the taught vocabulary. It tests the existing build without adding gameplay. Completing a task is not treated as proof of learning Chinese.

### Current prototype

The sequence is introduction → six vocabulary cards → ready screen → Task1 find the computer → Task2 catch the escaping red book, put it in the bag and release → completion. The six words are 书 book, 电脑 computer, 书包 school bag, 找到 find, 拿起 pick up and 放进 put into. Cards contain Chinese, pinyin, English and recorded word audio. A recorded Chinese instruction plays at the start of Task2.

Room objects do not display learning popups. The computer can be selected but cannot be carried away. The red book should remain stable on the desk before Task2, then move during Task2; the green book is a distractor. The pen and pencil case remain scene objects. Drawers, keys and unlocking are not part of the current task.

### Research questions

- Can participants understand the learning, ready and task sequence, and recognise success and retry feedback?

- Can participants complete Task2 using controller grabbing, spatial placement and release?

- Does immediate understanding of the six words change, and which English, pinyin or audio supports do participants actually use?

### Participants and equipment

Run one pilot, then recruit approximately five adult beginners in Chinese for individual sessions of 5–10 minutes. Exclude pilot data from the main summary. Record prior Chinese and VR experience, relevant access needs and ability to distinguish the two book colours. Recruitment and results must be recorded when they occur.

Plan approximately 9.5 minutes for the full protocol. Keep the same measures across participants and record actual duration, assistance and any omitted steps. Do not omit measures selectively to fit the time target.

Use the same headset, controller pair, build and starting position throughout. A facilitator observes the mirrored view and times tasks; use a separate note-taker if available. Record device models, build and volume. Keep existing language support consistent and do not switch to hand tracking during the study.

## 2 Session procedure

Explain that the prototype, not the participant, is being tested. Obtain agreement to participate and separate permission for screen or audio recording. If recording is declined, use de-identified written notes only. Participants may stop at any time.

| Stage | Time | Facilitator action |
| --- | --- | --- |
| Introduction and background | 0.5 min | Assign an ID; record Chinese and VR experience; explain stopping. |
| Initial vocabulary check | 1 min | Play six Chinese words and ask their meaning, without correction. |
| Control familiarisation | 0.5 min | Demonstrate pointing, selection, grab and release using the pen, not task answers. |
| Introduction and learning | 1 min | Let the participant view all six cards and reach the ready screen independently. |
| Task1 and Task2 | Up to 4 min | Time tasks separately: Task1 up to 60 seconds; Task2 up to 180 seconds. Record errors and help. A diagnostic restart is optional and, if needed, is logged separately after the main session. |
| Immediate vocabulary recheck | 1 min | Play the same words in a different order with identical scoring. |
| Ratings and interview | 1.5 min | Use page 6; record comfort and suggested improvements. |

### Opening script

“Please use this prototype as you normally would. When something does not work as you expect, that is useful for us to know. You may say what you are thinking. Tell me whenever you feel uncomfortable or want to stop.”

### Consistent assistance

Allow an independent attempt first. After about 20 seconds without progress, or a request for help, ask “What are you trying to do now?” Give an operational hint next if needed, and identify or demonstrate the answer only as a last resort. Log the time and level: H0 no help; H1 neutral prompt or replay; H2 operational hint; H3 direct answer or demonstration. H1–H3 do not count as fully independent completion.

A Task2 timeout at 180 seconds is a failed attempt. If a restart is useful for diagnosis, run it after the main timed session, retain the first result and log the repeat separately. Pause immediately for discomfort, tracking loss or a safety concern.

## 3 Task scripts and measures

### Learning and entering the tasks

Ask the participant to progress from the introduction through all six cards, use playback if needed, and reach the ready screen. Observe navigation, audio correspondence and the final transition without pressing buttons for them. Record learning time, revisits, playback counts and observed or reported use of English and pinyin.

### Task1 Find the computer

“Please complete the first task by following the instruction on screen.”

Start timing when Task1 is activated and its instruction is visible. Stop when correct computer selection produces success feedback. Success requires recognised selection and automatic transition to Task2. Log wrong objects, attempts to grab instead of select, instruction visibility and whether the computer stays in position. End the first attempt if it is not completed within 60 seconds, and document assistance.

### Task2 Pack the red book

“Please follow the second task’s text and spoken instruction.”

Start timing when the Task2 instruction appears and its recording begins; stop at completion feedback. The participant must catch the escaping red book, place it inside the bag and release it. Success requires the released target book to satisfy the interior check for about 0.4 seconds. A green book, an external placement or a held book must not pass.

Observe colour choice, understanding of the bag interior, grab and release, audio clarity and perceived lack of response. Ask about expectations without revealing detection rules. Log regrabs, misplaced or wrong objects, drops, help requests and feedback delays.

| Measure | Operational definition |
| --- | --- |
| First-attempt result | Independent success / assisted success / failure / technical interruption. |
| Task time | Seconds from the stated start to feedback; explain interrupted attempts. |
| Errors and help | Count wrong selections, placements outside the bag and waiting while holding; log H0–H3. |
| Support use | Record replays, English or pinyin use, and facilitator prompts. |
| Interaction and display issues | Log tracking loss, drifting objects, keyboard overlap, missing glyphs and invisible instructions. |

“Red” and “release” are needed in Task2 but are not among the six taught words. Check these concepts in the interview. Difficulties with them are instruction-design issues, not evidence that participants failed to learn the taught vocabulary.

## 4 Functional checks before sessions

Complete these developer checks before formal participant sessions. Keep them separate from user performance. Object physics and audio have changed recently, so earlier automated test results are not sufficient evidence for the current build.

| ID | Action | Expected result |
| --- | --- | --- |
| F01 | Cold-start and remain at introduction | Introduction appears; neither task starts early; room word popups remain absent. |
| F02 | View all six cards and play each clip | Order is book, computer, bag, find, pick up, put into; matching audio and readable glyphs. |
| F03 | Navigate back and forward; finish card six | Correct progress; ready screen follows; learning audio stops. |
| F04 | In Task1 select a book, then the computer | Retry is possible; computer selection starts Task2; computer neither falls nor can be carried. |
| F05 | Observe both books before Task2 | Books stay on clear desk space, away from the keyboard, without drifting or falling. |
| F06 | Enter Task2 and grab the red book | Instruction stays visible; narration plays; moving book can be grabbed. |
| F07 | Pack green book; hold red book in bag; release red book outside bag | None completes the task; feedback explains the relevant problem. |
| F08 | Put red book in bag and release | Completion follows a stable interior check; no drawer or key prerequisite. |
| F09 | Test timeout, restart and a dropped book | Correct timeout; one reset scene copy; dropped red book can be recovered. |

### Readiness and issue priority

Fix core-path blockers before formal sessions: a falling computer, ungrabbable red book, released book not completing inside the bag, or unreadable core text. Check controller input, text placement and sound on the target headset. Editor and automated checks are supplementary.

P1 blocks the task or creates a safety problem; P2 causes confusion, assistance or repeated failure; P3 is a presentation issue that does not prevent completion. Log stage, reproduction steps, expected and actual behaviour, and an evidence ID for each issue.

## 5 Observation sheet

Facilitator use only. Participant ID ______    Date ______    Build ______
Headset and controllers ____________________    Facilitator ______
Note-taker ______    Chinese experience __________________________
VR experience __________________________________________________

| Record | Task1 | Task2 |
| --- | --- | --- |
| Start and finish times | ________________ | ________________ |
| Seconds and first result | ________________ | ________________ |
| Wrong-object count | ________________ | ________________ |
| Outside placements / held waits | Not applicable | ________________ |
| Help level and count | ________________ | ________________ |
| Instruction replays / restarts | ________________ | ________________ |

### Vocabulary scoring

For both checks, play the same six isolated words without English or pinyin. Ask participants to explain or show the meaning. Allow one replay per word and accept equivalent explanations, not only exact English wording. Score correct = 1 and incorrect or unknown = 0. Give no answers during the initial check. Keep the answer column below hidden from participants.

| Word | Acceptable meaning | Before | After |
| --- | --- | --- | --- |
| 书 | book, or pointing to a book | ____ | ____ |
| 电脑 | computer, or pointing to it | ____ | ____ |
| 书包 | school bag, or pointing to it | ____ | ____ |
| 找到 | find, or an equivalent explanation | ____ | ____ |
| 拿起 | pick up, or demonstrating the action | ____ | ____ |
| 放进 | put into, or demonstrating the action | ____ | ____ |
| Total | 0–6 for each check | ____ / 6 | ____ / 6 |

Initial order: 书包, 找到, 书, 放进, 电脑, 拿起. Recheck order: 拿起, 电脑, 放进, 书, 找到, 书包. Use identical recordings and volume. Immediate change reflects short-term performance in this session, not long-term retention.

Key observations and quotations ___________________________________
__________________________________________________________________
Issue and evidence IDs ____________________________________________

## 6 Feedback and analysis

### Brief post-session ratings

For each statement, select 1–5: 1 strongly disagree, 2 disagree, 3 neutral, 4 agree, 5 strongly agree.

I understood what to do next.    1 [ ]  2 [ ]  3 [ ]  4 [ ]  5 [ ]

I could select the computer easily.    1 [ ]  2 [ ]  3 [ ]  4 [ ]  5 [ ]

I could control grabbing and place the red book in the bag.    1 [ ]  2 [ ]  3 [ ]  4 [ ]  5 [ ]

I could see, hear and understand the feedback.    1 [ ]  2 [ ]  3 [ ]  4 [ ]  5 [ ]

Manipulating objects helped me understand the Chinese words.    1 [ ]  2 [ ]  3 [ ]  4 [ ]  5 [ ]

### Interview questions

1. Which step was least clear, and what did you think the system wanted?

1. How did you know you had the correct book, correct position and a completed task?

1. Did you rely mainly on Chinese, English, pinyin, audio or object appearance?

1. Were “red” and “release” clear? What explanation would help?

1. Was anything uncomfortable or difficult to reach? What should change first?

### Analysis and design decisions

Report first-attempt results, seconds, errors and help for each task and participant. Summarise independent successes over actual attempts and the median and range of times. Separate technical interruptions from learning failures. Treat this small sample descriptively; do not claim statistical significance or general effectiveness.

Report each vocabulary score before and after, plus the difference, noting initial ceiling scores, prior Chinese experience and support use. There is no control group, and English, pinyin and repeated testing remain present. Scores cannot establish that VR, grabbing or Chinese-only instructions caused learning gains.

Suggested next-step criteria: resolve every P1 issue; prioritise a recurring confusion observed in at least two people. With a target of five participants, at least four independently completing both tasks can be a provisional readiness threshold, not a course grading requirement. Change one issue at a time and repeat the same procedure.

Store records under participant IDs rather than names. Retain recordings only with permission and manage or delete them under course requirements. Produce an issue list linking evidence, design decisions and questions for the next test.

## Research Summary

Five academic sources provide a basis for evaluating the Chinese VR learning prototype. Macedonia et al. (2023) found that grasping virtual objects supported immediate vocabulary retention among lower language-aptitude learners in a VR CAVE study. This finding supports exploring object manipulation in the prototype, although it does not establish the effectiveness of its controller-based interaction. Legault et al. (2019) found higher immediate Mandarin vocabulary recognition accuracy in immersive VR than in word–word paired-association learning, providing further support for investigating vocabulary learning through contextual experiences. Makransky and Petersen’s (2021) CAMIL framework explains how presence, agency and cognitive load may influence immersive learning. The evaluation therefore considers interaction usability and immediate vocabulary performance separately, rather than treating task completion as evidence of learning. Kao et al. (2021) found that the effectiveness of controller tutorial formats varied across VR game types. Their findings support examining whether participants understand the prototype’s controller instructions, while the test plan’s H0–H3 categories provide a separate record of facilitator assistance. Nielsen and Landauer’s (1993) usability problem-discovery model supports small, iterative evaluations focused on identifying interaction problems. A sample of approximately five participants is therefore appropriate for this formative evaluation, but does not establish learning effectiveness. Results are analysed descriptively, with attention to errors, assistance and the limitations of immediate vocabulary testing.
