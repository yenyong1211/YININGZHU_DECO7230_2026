# Duolingo XR — Immersive Chinese Learning

## Testing Plan for Interactive Prototype 1

## 1. Project Pitch

Duolingo XR is an immersive Chinese vocabulary learning experience that places language learning inside a virtual everyday environment.

Instead of learning vocabulary only through a traditional screen-based interface, users explore a virtual Study Room and interact with familiar objects such as a book, pen, desk, chair, and computer.

When users select an object, they can see its Chinese character, pinyin, and English meaning, and listen to its Chinese pronunciation.

The broader design principle is:

> **Language + Object + Action + Context → Better Understanding**

---

## 2. Testing Objective

The current prototype tests whether beginner learners can understand and use an object-based Chinese vocabulary learning interaction in a virtual Study Room.

### Main Assumption

Connecting Chinese vocabulary with recognisable 3D objects, text labels, and pronunciation may help beginner learners understand and remember the relationship between a Chinese word and its meaning.

### The test aims to discover:

- Whether participants understand that they can click objects in the environment to learn vocabulary.
- Whether the Chinese character, pinyin, and English translation on the vocabulary card are clear and useful.
- Whether pronunciation audio helps participants understand and remember a new Chinese word.
- Whether participants can associate Chinese vocabulary with the correct 3D objects after exploring them.
- Whether navigation, object selection, and vocabulary-card interaction are easy to understand.

This test focuses on the interactions currently implemented in Prototype 1 rather than evaluating the complete future Duolingo XR system.

---

## 3. Testing Methodology

The testing uses:

- **Task-Based Usability Testing**
- **Think-Aloud Protocol**

Participants are asked to complete several short tasks inside the Unity Study Room.

During the test, participants are encouraged to explain:

- what they are thinking;
- what they expect to happen;
- what they find confusing;
- why they make particular choices.

The Think-Aloud method is useful because the prototype is testing whether users understand the relationship between the virtual environment and the learning interface, not only whether they can technically complete the interaction.

---

## 4. Prototype Description

The prototype is a Unity 3D Study Room designed to demonstrate contextual Chinese vocabulary learning.

### Current Features

| Feature | Implementation |
|---|---|
| Virtual environment | 3D Study Room |
| Navigation | WASD movement |
| Camera control | Mouse-controlled viewing |
| Object selection | Mouse click / raycast |
| Book | 书 — shū — book |
| Pen | 笔 — bǐ — pen |
| Desk | 书桌 — shūzhuō — desk |
| Chair | 椅子 — yǐzi — chair |
| Computer | 电脑 — diànnǎo — computer |
| Vocabulary UI | Chinese + pinyin + English |
| Pronunciation | Listen button with Chinese audio |
| Contextual learning | Vocabulary is connected to the corresponding 3D object |

The current prototype mainly focuses on the **Learn Vocabulary** stage of the larger Duolingo XR experience.

---

## 5. Participants

The target was approximately **5 participants**, preferably people with little or no previous Chinese-learning experience.

Before starting, participants were asked:

> **Have you studied Chinese before?**

Possible responses:

- No previous Chinese experience
- Some previous experience
- Intermediate or above

Participants with more Chinese experience can still provide useful usability feedback, but their vocabulary-recognition results should be interpreted separately.

---

## 6. Testing Setup

Before each participant:

1. Open the `StudyRoom` scene in Unity.
2. Start the prototype in Play Mode.
3. Reset the player/camera to the starting position.
4. Make sure the vocabulary card is initially hidden.
5. Check that all five learning objects are clickable.
6. Confirm that each object displays the correct vocabulary.
7. Confirm that the Listen button plays the correct pronunciation.
8. Prepare a timer.
9. Prepare the observation sheet.

Participants are not initially told exactly which objects are interactive.

This helps evaluate whether the interaction is discoverable.

---

## 7. Testing Process

**Target duration: approximately 5 minutes per participant.**

### Step 1 — Introduction
**Approx. 30 seconds**

Tell the participant:

> “This is a prototype for learning beginner Chinese vocabulary in a virtual Study Room. Please explore the room and complete the tasks I give you. While using it, please say aloud what you are thinking, what you expect to happen, and anything you find confusing.”

Do not immediately explain how to open vocabulary cards.

---

### Step 2 — Exploration and Interaction
**Approx. 1 minute**

Instruction:

> “Please explore the Study Room and see what you can interact with.”

Observe:

- Do they understand WASD movement?
- Do they understand mouse camera movement?
- Do they attempt to click objects?
- Which object do they click first?
- Do they recognise that clicking an object opens a vocabulary card?
- Do they accidentally click UI elements or other objects?
- Do they need help from the facilitator?

If they cannot discover the interaction after approximately 30 seconds, give the hint:

> “Try clicking one of the objects in the room.”

Record any facilitator hints.

---

### Step 3 — Learn the Vocabulary
**Approx. 1.5 minutes**

Ask:

> “Please find and learn the Chinese words for the five objects in the room.”

| Object | Chinese | Pinyin | English |
|---|---|---|---|
| Book | 书 | shū | book |
| Pen | 笔 | bǐ | pen |
| Desk | 书桌 | shūzhuō | desk |
| Chair | 椅子 | yǐzi | chair |
| Computer | 电脑 | diànnǎo | computer |

Ask the participant to use the **Listen** button at least once.

Observe:

- whether the vocabulary card is readable;
- whether participants understand the Chinese, pinyin, and English information;
- whether the pronunciation button is noticeable;
- whether participants replay pronunciation;
- whether switching between objects feels intuitive.

---

### Step 4 — Simple Recall Test
**Approx. 1 minute**

Ask the participant to identify several words without reopening the vocabulary card.

Example questions:

> “Which object do you think ‘书 — shū’ refers to?”

> “Which object do you think ‘书桌 — shūzhuō’ refers to?”

> “Which object do you think ‘电脑 — diànnǎo’ refers to?”

Ask the participant to point to or name the corresponding object.

Record:

- ✓ Correct
- ✕ Incorrect
- ? Unsure

This gives an initial measure of short-term word-object association.

---

### Step 5 — Post-Test Questions
**Approx. 1 minute**

#### Question 1
**How easy was it to understand how to interact with objects?**

- 1 — Very difficult
- 2 — Difficult
- 3 — Neutral
- 4 — Easy
- 5 — Very easy

#### Question 2
**How useful was seeing Chinese, pinyin, and English together?**

- 1 — Not useful
- 2 — Slightly useful
- 3 — Neutral
- 4 — Useful
- 5 — Very useful

#### Question 3
**How useful was the pronunciation audio?**

- 1 — Not useful
- 2 — Slightly useful
- 3 — Neutral
- 4 — Useful
- 5 — Very useful

#### Question 4
**Did connecting the word to a 3D object help you remember it? Why or why not?**

#### Question 5
**Was anything confusing about the vocabulary card or controls?**

#### Question 6
**What would you change or improve?**

---

## 8. Data Collection

For each participant, both quantitative and qualitative data are collected.

| Measure | Data |
|---|---|
| Chinese experience | None / Some / Experienced |
| Time until first successful interaction | Seconds |
| Number of interaction hints | Number |
| Objects successfully opened | /5 |
| Listen button discovered | Yes / No |
| Pronunciation used | Number of times |
| Vocabulary recognition | Correct / Incorrect / Unsure |
| Navigation difficulty | Notes |
| Interaction ease | 1–5 |
| Vocabulary UI usefulness | 1–5 |
| Audio usefulness | 1–5 |
| Learning time | Minutes / seconds |
| Participant comments | Notes |

---

## 9. Observation Sheet

| Participant | First Interaction | Hints | Objects /5 | Word 1 | Word 2 | Word 3 | UI /5 | Audio /5 | Notes |
|---|---:|---:|---:|---|---|---|---:|---:|---|
| P1 |  |  |  |  |  |  |  |  |  |
| P2 |  |  |  |  |  |  |  |  |  |
| P3 |  |  |  |  |  |  |  |  |  |
| P4 |  |  |  |  |  |  |  |  |  |
| P5 |  |  |  |  |  |  |  |  |  |

---

## 10. Success Criteria

The prototype will be considered successful if:

- Most participants discover how to select learning objects with little assistance.
- Most participants successfully open and explore the vocabulary objects.
- Participants understand that the Chinese word corresponds to the selected 3D object.
- Most participants correctly recall at least **2 out of 3** tested vocabulary-object relationships.
- Participants rate the vocabulary card positively.
- Participants rate the pronunciation audio positively.
- Participants can explain the relationship between the Chinese word and the virtual object.
- Navigation or UI issues do not prevent participants from completing the main learning task.

The aim is not to demonstrate long-term Chinese-language learning after a short test. Instead, the test evaluates whether contextual object-based vocabulary learning is understandable and usable enough to develop further.

---

## 11. Current Testing Focus

### This prototype currently tests:

- Study Room navigation
- Discoverability of clickable objects
- Contextual object-based vocabulary learning
- Chinese / pinyin / English presentation
- Pronunciation audio
- Short-term vocabulary-object association

### This prototype does not currently test:

- XR controller interaction
- Grabbing objects
- “请拿起书”
- Full recognition mode
- Visual hints or arrows
- Speech recognition

These features remain possible directions for later prototype iterations.