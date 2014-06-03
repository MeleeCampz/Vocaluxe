using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;

class PhidgetsController
{
    private static PhidgetsController instance;
    public static PhidgetsController Instance
    {
        get
        {
            if (instance == null)
            {

                instance = new PhidgetsController();
            }
            return instance;
        }
    }

    private InterfaceKit interfaceKit_;

    private PhidgetsController()
    {
    }

    public void Init()
    {
        interfaceKit_ = new InterfaceKit();
        interfaceKit_.open();
        interfaceKit_.waitForAttachment();

        state_ = State.idle;
    }

    public void Cleanup()
    {
        interfaceKit_.close();
    }

    private float lastTime_;
    private float timeCount_;

    private State state_;

    private const float duration_Forward_   = 0.2f;
    private const float duration_Wait_      = 0.5f;
    private const float duration_Backward_  = 0.2f;

    public void Update(float time)
    {
        if (interfaceKit_.Attached)
        {
            float deltaTime = time - lastTime_;
            timeCount_ += deltaTime;

            switch (state_)
            {
                case State.idle:
                    interfaceKit_.outputs[0] = false;
                    interfaceKit_.outputs[1] = false;
                    break;

                case State.forwarding:
                    interfaceKit_.outputs[0] = true;
                    interfaceKit_.outputs[1] = false;
                    if (timeCount_ > duration_Forward_)
                    {
                        timeCount_ = 0.0f;
                        state_ = State.waiting;
                    }

                    break;

                case State.waiting:
                    interfaceKit_.outputs[0] = false;
                    interfaceKit_.outputs[1] = false;
                    if (timeCount_ > duration_Wait_)
                    {
                        timeCount_ = 0.0f;
                        state_ = State.backwarding;
                    }
                    break;

                case State.backwarding:
                    interfaceKit_.outputs[0] = false;
                    interfaceKit_.outputs[1] = true;
                    if (timeCount_ > duration_Backward_)
                    {
                        timeCount_ = 0.0f;
                        state_ = State.idle;
                    }

                    break;
            }

        }
        else
        {
            interfaceKit_.waitForAttachment();
        }
        lastTime_ = time;
    }

    public void StartShower()
    {
        state_ = State.forwarding;
        timeCount_ = 0.0f;
    }


    private enum State { idle, forwarding, waiting, backwarding }
}
