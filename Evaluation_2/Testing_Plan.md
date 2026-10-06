# Duolingo XR — IP2a  — Testing Plan

## Chinese Learning Room

---

## 1. Prototype Overview

This prototype is a VR Chinese learning experience for beginner Chinese learners.

Users first learn five basic Chinese words through learning cards. Each card shows:

- Chinese word
- Pinyin
- English meaning
- Audio pronunciation

After learning, users enter a VR room. They can freely explore and interact with objects. Clicking an object shows its Chinese name.

When users are ready, they press the **START TASK** button to begin the mission.

The mission contains two tasks:

1. **找到电脑** — Find the computer.
2. **拿起书** — Pick up the book.

---

## 2. Testing Aim

The testing aims to understand:

1. Whether users can learn and remember the five Chinese words.
2. Whether the learning cards and audio are easy to understand and use.
3. Whether users understand that they can explore and interact with objects in the VR room.
4. Whether users can understand Chinese task instructions without English translation.
5. Whether VR interactions, especially selecting objects and grabbing the book, are easy to complete.
6. Which parts of the experience are confusing, difficult, or enjoyable.

---

## 3. Participants

**Target number:** 5–6 participants  
**Suggested testing time:** 8–10 minutes per participant

Try to include different participant backgrounds:

| Participant Characteristic | Why |
|---|---|
| No Chinese experience | Test the real beginner learning experience |
| Basic Chinese experience | See whether some previous Chinese knowledge affects task completion |
| Little / no VR experience | Test whether the interaction is intuitive |
| Some VR experience | Help separate VR interaction problems from Chinese understanding problems |

---

## 4. Equipment

- Meta Quest headset
- VR controllers
- Unity prototype
- Testing observation sheet
- Pen / laptop for notes
- Headset video streaming if available

---

## 5. Testing Procedure

### Before Testing

Researcher introduction:

> Hi, thank you for helping me test my prototype.  
> This is a VR experience for beginner Chinese learning.  
> Please use the prototype as naturally as possible.  
> If anything is confusing, please tell me.  
> I am testing the prototype, not you.

### Pre-test Questions

1. **Have you learned Chinese before?**
2. **Have you used a VR headset before?**
3. **How confident are you using VR controllers? (1–5)**

---

## 6. Testing Stage 1 — Vocabulary Learning

Participants first enter the vocabulary learning stage.

### Vocabulary

| Chinese | Pinyin | English |
|---|---|---|
| 书 | shū | book |
| 桌子 | zhuōzi | table |
| 电脑 | diànnǎo | computer |
| 找到 | zhǎodào | find |
| 拿起 | náqǐ | pick up |

Participants can:

- view the Chinese word;
- view pinyin;
- view the English meaning;
- press the **Audio Button** to hear pronunciation;
- use **Next / Previous** to switch cards.

### Researcher Observations

Observe whether the participant:

- knows how to click **Start Learning**;
- understands how to use **Next**;
- tries **Previous**;
- actively uses the **Audio Button**;
- finds any word difficult to remember;
- has difficulty seeing any buttons or text;
- can use the Controller Ray easily.

### Important

Do not help immediately.

If the participant only hesitates for a few seconds, wait and observe. Only provide help if they are clearly unable to continue.

---

## 7. Testing Stage 2 — Explore the Room

After the learning stage, the participant enters the VR room.

They do **not** need to begin the task immediately. They can freely explore first.

Clicking objects can show their Chinese word cards, for example:

- 书
- 桌子
- 电脑

The room also contains the **START TASK** button.

### Researcher Instruction

Say only:

> You can explore the room first. Start the task when you are ready.

Do **not** say:

> Click the book.  
> Click the computer.

The aim is to observe whether the participant discovers the interaction independently.

### Observe

Record whether the participant:

- explores the room voluntarily;
- tries to click objects;
- discovers that objects are interactive;
- notices the Chinese word card after clicking;
- clicks multiple objects;
- finds the **START TASK** button;
- needs help.

The participant does not need to explore everything before pressing **START TASK**.

---

## 8. Testing Stage 3 — In-game Task 1

### Task Instruction

> **找到电脑**

No English translation is shown in the prototype.

The participant needs to select the computer.

### Success Condition

The player selects the computer.

### Correct Response

Display:

> **太棒了！**  
> **Great job!**

Then continue to Task 2.

### Incorrect Response

If the player selects the wrong object, display:

> **再试一次**  
> **Try again**

The player remains in Task 1.

### Data to Record

| Measure | Record |
|---|---|
| First attempt correct? | Yes / No |
| Wrong object selected | ______ |
| Number of wrong attempts | ______ |
| Needed help? | Yes / No |
| Understood **找到电脑**? | Yes / No / Unsure |
| Interaction problem | ______ |

This task helps identify whether the participant remembers the Chinese instruction or is only guessing.

---

## 9. Testing Stage 4 — In-game Task 2

### Task Instruction

> **拿起书**

The participant needs to:

1. find the book;
2. use the VR controller;
3. grab the book;
4. lift the book.

### Success Condition

The player successfully grabs and lifts the book.

### Correct Response

Display:

> **太棒了！**  
> **Great job!**

### Data to Record

| Measure | Record |
|---|---|
| Understood **拿起书**? | Yes / No / Unsure |
| Found the correct object? | Yes / No |
| Tried clicking instead of grabbing? | Yes / No |
| Successfully grabbed the book? | Yes / No |
| Needed help? | Yes / No |
| Controller difficulty | ______ |

This task tests both:

**Chinese understanding + VR physical interaction**

---

## 10. Post-test Questions

Keep the interview short.

1. **Which Chinese words do you remember?**
2. **What do you think “找到电脑” means?**
3. **What do you think “拿起书” means?**
4. **Which part was the most confusing?**
5. **Which part did you enjoy the most?**
6. **What would you change or add?**

### Ratings

**How easy was the prototype to use?**

- 1 = Very difficult
- 2 = Difficult
- 3 = Neutral
- 4 = Easy
- 5 = Very easy

**How enjoyable was the experience?**

- 1 = Not enjoyable
- 2 = Slightly enjoyable
- 3 = Neutral
- 4 = Enjoyable
- 5 = Very enjoyable

---

## 11. Data to Collect

Collect both what participants **say** and what they **actually do**.

| Type | Data |
|---|---|
| Learning | Number of words remembered; understanding of task instructions |
| Performance | Task completion; number of errors; help needed |
| Interaction | Controller, Ray, Button, and Grab problems |
| Experience | Confusing parts, enjoyable parts, suggestions |

Do not only record comments.

For example, a participant may say:

> “Yeah, it was easy.”

But they may have needed three hints during the task. Behavioural data is therefore also important.

---

## 12. Success Criteria

For 5–6 participants, the initial targets are:

| Measure | Target |
|---|---:|
| Complete Learning Stage | ≥ 5/6 |
| Find START TASK without help | ≥ 4/6 |
| Complete Task 1 | ≥ 4/6 |
| Task 1 first attempt correct | ≥ 3/6 |
| Complete Task 2 | ≥ 4/6 |
| Remember at least 3 words | ≥ 4/6 |
| Average ease score | ≥ 3/5 |
| Average enjoyment score | ≥ 3/5 |

These are not strict pass/fail requirements. If the prototype does not meet a target, the result can help identify the next design iteration.

---

## 13. Testing Record Sheet

### Participant Information

- **Participant ID:** P___
- **Chinese experience:** None / Beginner / Intermediate
- **VR experience:** None / Some / Experienced
- **VR confidence:** ___ / 5

### Stage 1 — Vocabulary Learning

| Observation | Result |
|---|---|
| Started learning without help | Yes / No |
| Used Next correctly | Yes / No |
| Used Previous correctly | Yes / No |
| Used Audio button | Yes / No |
| Needed help | Yes / No |
| Difficult word(s) | ______ |
| Notes | ______ |

### Stage 2 — Explore

| Observation | Result |
|---|---|
| Explored voluntarily | Yes / No |
| Clicked objects | Yes / No |
| Noticed object word cards | Yes / No |
| Found START TASK | Yes / No |
| Needed help | Yes / No |
| Notes | ______ |

### In-game Task 1 — 找到电脑

| Observation | Result |
|---|---|
| First attempt correct | Yes / No |
| Wrong attempts | ______ |
| Wrong objects selected | ______ |
| Needed help | Yes / No |
| Understood instruction | Yes / No / Unsure |
| Notes | ______ |

### In-game Task 2 — 拿起书

| Observation | Result |
|---|---|
| Found the book | Yes / No |
| Understood instruction | Yes / No / Unsure |
| Tried clicking instead of grabbing | Yes / No |
| Successfully grabbed | Yes / No |
| Needed help | Yes / No |
| Notes | ______ |

### Post-test

- **Words remembered:** ______
- **Meaning of 找到电脑:** ______
- **Meaning of 拿起书:** ______
- **Most confusing part:** ______
- **Most enjoyable part:** ______
- **Suggested improvement:** ______
- **Ease:** ___ / 5
- **Enjoyment:** ___ / 5
